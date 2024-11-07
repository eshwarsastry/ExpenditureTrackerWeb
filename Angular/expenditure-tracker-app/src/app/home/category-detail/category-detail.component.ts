import { OnInit , Component } from '@angular/core';
import { AddTransactionCategoryFormComponent } from '../forms/add-transaction-category-form/add-transaction-category-form.component';
import { MatDialog } from '@angular/material/dialog';
import { TransactionCategory } from '../../shared/interfaces/interfaces';
import { Router } from '@angular/router';
import { SharedService } from '../../shared/services/shared.service';
import { TransactionCategoryService } from '../../shared/services/transaction-category.service';

@Component({
  selector: 'app-category-detail',
  templateUrl: './category-detail.component.html',
  styleUrl: './category-detail.component.css'
})
export class CategoryDetailComponent implements OnInit {
  constructor(private router: Router,
    private dialog: MatDialog,
    private sharedService: SharedService,
    private transactionCategoryService: TransactionCategoryService) { }

  displayedColumns: string[] = ['name', 'description'];
  loggedInUserData: any;
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  categoryTableData: TransactionCategory[] = [];

  ngOnInit() {
    this.sharedService.data$.subscribe((data) => {
      this.loggedInUserData = data;
      this.transactionCategoryService.getAllTransactionCategories(this.loggedInUserData.userId).subscribe((response) => {
        this.categoryTableData = response;
      });
    });
    
  }

  openAddTransactionCategoryDialog() {
    const dialogRef = this.dialog.open(AddTransactionCategoryFormComponent, {
      width: '400px',  
      data: this.loggedInUserData         
    });

    
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Logic after the dialog is closed, if needed
      }
    });
  }
}

