import { OnInit, Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { TransactionCategory } from '../../shared/interfaces/interfaces';
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
    private transactionCategoryService: TransactionCategoryService)
  {
     
  }

  displayedColumns: string[] = ['id', 'name', 'description', 'transactionType','edit','delete'];
  loggedInUserId: number = 0; //Fetch login user data to be used for identification in backend APIs.
  editable = false;

  //Values used for the category table.
  categories: TransactionCategory[] = []
  categoryTableData = new MatTableDataSource(this.categories);
  transactionCategoryRow: TransactionCategory = this.initializeTransactionCategory();

  ngOnInit() {
    this.sharedService.userIdSubject.subscribe((data) => {
      this.loggedInUserId = data;
      this.transactionCategoryService.getAllTransactionCategories(this.loggedInUserId).subscribe((response) => {
        this.categoryTableData.data = response;
      });
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

  addTransactionCategory() {
    this.sharedService.openAddTransactionCategoryDialog(this.loggedInUserId, this.editable, this.transactionCategoryRow)
      .subscribe(result => {
        // Logic after the dialog is closed, if needed
        this.transactionCategoryRow.id = 0; //reset value.
        this.transactionCategoryRow = this.initializeTransactionCategory();
        this.transactionCategoryService.getAllTransactionCategories(this.loggedInUserId).subscribe((response) => {
          this.categoryTableData.data = response;
        });
      });
  }

  editTransactionCategory(rowData: TransactionCategory) {
    this.transactionCategoryRow = rowData;
    this.editable = true;
    this.addTransactionCategory();
  }

  removeTransactionCategory(categoryId: number) {
    this.transactionCategoryService.deleteCategory(categoryId).subscribe(() => {
      this.transactionCategoryService.getAllTransactionCategories(this.loggedInUserId).subscribe((response) => {
        this.categoryTableData.data = response;
      });
    }); 
  }
}

