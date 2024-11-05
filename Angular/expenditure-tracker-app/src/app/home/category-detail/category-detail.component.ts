import { OnInit , Component } from '@angular/core';
import { AddTransactionCategoryFormComponent } from '../forms/add-transaction-category-form/add-transaction-category-form.component';
import { MatDialog } from '@angular/material/dialog';
import { TransactionCategory } from '../../shared/interfaces/interfaces';
import { Router } from '@angular/router';
import { SharedService } from '../../shared/services/shared.service';

@Component({
  selector: 'app-category-detail',
  templateUrl: './category-detail.component.html',
  styleUrl: './category-detail.component.css'
})
export class CategoryDetailComponent implements OnInit {
  constructor(private router: Router,
    private dialog: MatDialog,
    private sharedService: SharedService) { }

  displayedColumns: string[] = ['name', 'description'];
  sharedData: any;
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  data: TransactionCategory[] = [];

  ngOnInit() {
    this.sharedService.data$.subscribe((data) => {
      this.sharedData = data; // Receive data whenever it’s updated
    });
  }

  openAddTransactionCategoryDialog() {
    const dialogRef = this.dialog.open(AddTransactionCategoryFormComponent, {
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

