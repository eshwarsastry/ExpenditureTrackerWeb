import { OnInit, Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { AddTransactionCategoryFormComponent } from '../forms/add-transaction-category-form/add-transaction-category-form.component';
import { CategoryDialogData, TransactionCategory } from '../../shared/interfaces/interfaces';
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
  transactionCategoryRow: TransactionCategory = {
    id : 0,
    transactionType_Id: 0,
    user_Id: 0,
    name: '',
    description:''
  }; //Fields to be passed to the dialog form. Pass a defualt value at times.
  loggedInUserData: any; //Fetch login user data to be used for identification in backend APIs.
  
  editable = false;

  //Values used for the category table.
  categories: TransactionCategory[] = []
  categoryTableData = new MatTableDataSource(this.categories);


  // Define a mapping for transaction type to display value
  typeDisplayMap: { [key: number]: string } = {
    1: 'Income',
    2: 'Expense'
  };

  ngOnInit() {
    this.sharedService.data$.subscribe((data) => {
      this.loggedInUserData = data;
      this.transactionCategoryService.getAllTransactionCategories(this.loggedInUserData.userId).subscribe((response) => {
        this.categoryTableData.data = response;
      });
    });
  }

  getTypeDisplayValue(type: number): string {
    return this.typeDisplayMap[type] || ''; // Default to the raw value if not found
  }

  addTransactionCategory() {
    const dialogData: CategoryDialogData = {
      transactionCategoryRow: this.transactionCategoryRow,
      loggedInUserData: this.loggedInUserData,
      editable: this.editable
    };
    const dialogRef = this.dialog.open(AddTransactionCategoryFormComponent, {
      width: '400px',
      data: dialogData
    });


    dialogRef.afterClosed().subscribe(result => {
      // Logic after the dialog is closed, if needed
      this.transactionCategoryRow.id = 0; //reset value.
      this.transactionCategoryService.getAllTransactionCategories(this.loggedInUserData.userId).subscribe((response) => {
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
      this.transactionCategoryService.getAllTransactionCategories(this.loggedInUserData.userId).subscribe((response) => {
        this.categoryTableData.data = response;
      });
    }); 
  }
}

