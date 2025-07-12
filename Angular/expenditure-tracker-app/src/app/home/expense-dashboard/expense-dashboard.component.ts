import { OnInit, Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { AddTransactionCategoryFormComponent } from '../forms/add-transaction-category-form/add-transaction-category-form.component';
import { AddTransactionExpenseFormComponent } from '../forms/add-transaction-expense-form/add-transaction-expense-form.component';
import { SharedService } from '../../shared/services/shared.service';
import { TransactionService } from '../../shared/services/transaction.service';
import { CategoryDialogData, ExpenseDialogData, LoginResponse, TransactionCategory, Transactions } from '../../shared/interfaces/interfaces';

@Component({
  selector: 'app-expense-dashboard',
  templateUrl: './expense-dashboard.component.html',
  styleUrl: './expense-dashboard.component.css'
})

export class ExpenseDashboardComponent implements OnInit {
  constructor(private router: Router,
    private dialog: MatDialog,
    private sharedService: SharedService,
    private transactionExpenseService: TransactionService) { }

  transactionCategory: TransactionCategory = this.initializeTransactionCategory();
  transaction: Transactions = this.initializeTransactions();
  transactions: Transactions[] = [];
  recentTransactions: Transactions[] = [];
  loggedInUserId: number = 0; //Fetch login user data to be used for identification in backend APIs.
  editable = false;
  isLoading = true;
  hasOverflow = false;

  ngOnInit() {
    this.sharedService.userIdSubject.subscribe((data) => {
      this.loggedInUserId = data;
      this.fetchRecentTransactions();
    });
  }

  initializeTransactionCategory(): TransactionCategory {
    return {
      id: 0,
      transactionType_Id: 0,
      transactionType_Name: '',
      user_Id: 0,
      name: '',
      description: ''
    }; //Fields to be passed to the dialog form. Pass a defualt value at times.
  }

  initializeTransactions(): Transactions {
    return {
      id: 0,
      transactionDate: new Date(),
      user_Id: 0,
      category_Id: 0,
      category_Name: '',
      transactionType_Id: 0,
      transactionType_Name: '',
      amount: 0,
      note: ''
    }; //Fields to be passed to the dialog form. Pass a defualt value at times.
  }

  openAddTransactionCategoryDialog() {
    const dialogData: CategoryDialogData = {
      transactionCategoryRow: this.transactionCategory,
      loggedInUserId: this.loggedInUserId,
      editable: this.editable
    };
    const dialogRef = this.dialog.open(AddTransactionCategoryFormComponent, {
      width: '400px',
      data: dialogData
    });


    dialogRef.afterClosed().subscribe(result => {
      // Logic after the dialog is closed, if needed
      this.transactionCategory.id = 0; //reset value.
      this.transactionCategory = this.initializeTransactionCategory();
    });
  }

  openAddTransactionExpenseDialog() {
    const dialogData: ExpenseDialogData = {
      transactionRow: this.transaction,
      loggedInUserId: this.loggedInUserId,
      editable: this.editable
    };
    const dialogRef = this.dialog.open(AddTransactionExpenseFormComponent, {
      width: '400px',  // Optional: set width of the dialog
      data: dialogData         // Optional: pass data to dialog if needed
    });

    // Handle dialog close (optional)
    dialogRef.afterClosed().subscribe(result => {
      // Logic after the dialog is closed, if needed
      this.transaction.id = 0; //reset value.
      this.transaction = this.initializeTransactions();
      this.fetchRecentTransactions();
    });
  }

  fetchRecentTransactions(): void {
    this.transactionExpenseService.getRecentTransactions(this.loggedInUserId).subscribe(
      (data) => {
        this.recentTransactions = data;
        this.isLoading = false;
        // Check for overflow after data loads
        setTimeout(() => this.checkOverflow(), 100);
      },
      (error) => {
        console.error('Error fetching transactions:', error);
        this.isLoading = false;
      }
    );
  }

  checkOverflow(): void {
    const transactionsSection = document.querySelector('.transactions-section');
    if (transactionsSection) {
      const ul = transactionsSection.querySelector('ul');
      if (ul) {
        this.hasOverflow = ul.scrollHeight > ul.clientHeight;
      }
    }
  }
}
