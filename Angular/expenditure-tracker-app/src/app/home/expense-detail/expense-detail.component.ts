import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { AddTransactionExpenseFormComponent } from '../forms/add-transaction-expense-form/add-transaction-expense-form.component';
import { ExpenseDialogData, Transactions } from '../../shared/interfaces/interfaces';
import { SharedService } from '../../shared/services/shared.service';
import { TransactionService } from '../../shared/services/transaction.service';


@Component({
  selector: 'app-expense-detail',
  templateUrl: './expense-detail.component.html',
  styleUrl: './expense-detail.component.css'
})
export class ExpenseDetailComponent implements OnInit {
  constructor(private router: Router,
    private dialog: MatDialog,
    private sharedService: SharedService,
    private transactionExpenseService: TransactionService) { }

  displayedColumns: string[] = ['id', 'date', 'category', 'transactionType', 'amount', 'note', 'edit', 'delete'];
  //Values used for the transaction table.
  transactions: Transactions[] = [];
  transactionTableData = new MatTableDataSource(this.transactions);

  transactionRow: Transactions = this.initializeTransactions();

  loggedInUserId: number = 0; //Fetch login user data to be used for identification in backend APIs.

  editable = false;

  ngOnInit(): void {
    this.sharedService.userIdSubject.subscribe((data) => {
      this.loggedInUserId = data;
      this.transactionExpenseService.getAllTransactions(this.loggedInUserId).subscribe((response) => {
        this.transactionTableData.data = response;
      });
    });
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

  addTransactionExpense() {
    const dialogData: ExpenseDialogData = {
      transactionRow: this.transactionRow,
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
      this.transactionRow.id = 0; //reset value.
      this.transactionRow = this.initializeTransactions();
      this.transactionExpenseService.getAllTransactions(this.loggedInUserId).subscribe((response) => {
        this.transactionTableData.data = response;
      });
    });
  }

  editTransaction(rowData: Transactions) {
    this.transactionRow = rowData;
    this.editable = true;
    this.addTransactionExpense();
  }

  removeTransaction(transactionId: number) {
    this.transactionExpenseService.deleteTransaction(transactionId).subscribe(() => {
      this.transactionExpenseService.getAllTransactions(this.loggedInUserId).subscribe((response) => {
        this.transactionTableData.data = response;
      });
    });
  }
}
