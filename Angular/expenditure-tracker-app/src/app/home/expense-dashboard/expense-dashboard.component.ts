import { OnInit, Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { AddTransactionCategoryFormComponent } from '../forms/add-transaction-category-form/add-transaction-category-form.component';
import { AddTransactionExpenseFormComponent } from '../forms/add-transaction-expense-form/add-transaction-expense-form.component';
import { SharedService } from '../../shared/services/shared.service';

@Component({
  selector: 'app-expense-dashboard',
  templateUrl: './expense-dashboard.component.html',
  styleUrl: './expense-dashboard.component.css'
})

export class ExpenseDashboardComponent implements OnInit {
  constructor(private router: Router,
    private dialog: MatDialog,
    private sharedService: SharedService) { }

  sharedData: any;

  ngOnInit() {
    this.sharedService.data$.subscribe((data) => {
      this.sharedData = data; // Receive data whenever it’s updated
    });
  }
  
  openAddTransactionCategoryDialog() {
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
