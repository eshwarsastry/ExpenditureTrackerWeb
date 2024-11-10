import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { AddTransactionExpenseFormComponent } from '../forms/add-transaction-expense-form/add-transaction-expense-form.component';
import { Transactions } from '../../shared/interfaces/interfaces';
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
    private transactionExpenseService: TransactionService ) { }

  displayedColumns: string[] = ['id', 'date', 'category','transactionType','amount', 'note'];
  //Values used for the transaction table.
  transactions: Transactions[] = [];
  transactionTableData = new MatTableDataSource(this.transactions);

  transactionRow: Transactions = {
    id: 0,
    transactionDate: new Date(),
    user_Id: 0,
    category_Id:0,
    amount: 0,
    note: ''
  }; //Fields to be passed to the dialog form. Pass a defualt value at times.
  loggedInUserData: any; //Fetch login user data to be used for identification in backend APIs.

  editable = false;


  // Define a mapping for transaction type to display value
  typeDisplayMap: { [key: number]: string } = {
    1: 'Income',
    2: 'Expense'
  };

  getTypeDisplayValue(type: number): string {
    return this.typeDisplayMap[type] || ''; // Default to the raw value if not found
  }

  getCategoryDisplayValue(category: number): string {

    return this.typeDisplayMap[category] || ''; // Default to the raw value if not found
  }

  ngOnInit(): void {
    this.sharedService.data$.subscribe((data) => {
      this.loggedInUserData = data;
      this.transactionExpenseService.getAllTransactions(this.loggedInUserData.userId).subscribe((response) => {
        this.transactionTableData.data = response;
      });
    });
  }

  addTransactionExpense() {
    const dialogRef = this.dialog.open(AddTransactionExpenseFormComponent, {
      width: '400px',  // Optional: set width of the dialog
      data: {}         // Optional: pass data to dialog if needed
    });

    // Handle dialog close (optional)
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Logic after the dialog is closed, if needed
      }
    });
  }

  editTransaction(rowData: Transactions) {
    this.transactionRow = rowData;
    this.editable = true;
    this.addTransactionExpense();
  }

  removeTransaction(transactionId: number) {
    //this.transactionCategoryService.deleteCategory(categoryId).subscribe(() => {
    //  this.transactionCategoryService.getAllTransactionCategories(this.loggedInUserData.userId).subscribe((response) => {
    //    this.categoryTableData.data = response;
    //  });
    //});
  }
}
