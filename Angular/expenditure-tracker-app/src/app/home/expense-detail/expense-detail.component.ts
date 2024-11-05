import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { AddTransactionExpenseFormComponent } from '../forms/add-transaction-expense-form/add-transaction-expense-form.component';
import { Transactions } from '../../shared/interfaces/interfaces';


@Component({
  selector: 'app-expense-detail',
  templateUrl: './expense-detail.component.html',
  styleUrl: './expense-detail.component.css'
})
export class ExpenseDetailComponent {
  constructor(private router: Router,
    private dialog: MatDialog) { }

  displayedColumns: string[] = ['Date', 'Amount', 'Note', 'Category'];
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  data: Transactions[] = [];

  openAddTransactionDialog() {
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
}
