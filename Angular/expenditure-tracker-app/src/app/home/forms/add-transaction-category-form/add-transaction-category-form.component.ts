import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TransactionCategory, TransactionType } from '../../../shared/interfaces/interfaces';
import { TransactionTypeService } from '../../../shared/services/transaction-type.service';
import { TransactionCategoryService } from '../../../shared/services/transaction-category.service';
import { User } from '../../../shared/interfaces/user';

@Component({
  selector: 'app-add-transaction-category-form',
  templateUrl: './add-transaction-category-form.component.html',
  styleUrl: './add-transaction-category-form.component.css'
})

export class AddTransactionCategoryFormComponent implements OnInit {
  transactionCategoryForm: FormGroup;
  transactionTypes: TransactionType[] = [];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddTransactionCategoryFormComponent>,
    private transactionTypeService: TransactionTypeService,
    private transactionCategoryService: TransactionCategoryService,
    @Inject(MAT_DIALOG_DATA) public dailogData: any
  ) {
    this.transactionCategoryForm = this.fb.group({
      id: [''],
      transactionType: ['', Validators.required],
      categoryName: ['', Validators.required],
      categoryDescription: ['', Validators.required]
    });
  }


  ngOnInit(): void {
    this.transactionTypeService.getAllTransactionTypes().subscribe((response) => {
      this.transactionTypes = response;
    });
  }

  onSave() {
    if (this.transactionCategoryForm.valid) {
      const transactionCategory: TransactionCategory = {
        id: this.dailogData.transactionCategoryRow.id ?? 0,
        user_Id: this.dailogData.loggedInUserData.userId,
        transactionType_Id: this.transactionCategoryForm.get('transactionType')?.value,
        name: this.transactionCategoryForm.get('categoryName')?.value,
        description: this.transactionCategoryForm.get('categoryDescription')?.value
      };
      this.transactionCategoryService.addCategory(transactionCategory).subscribe((response) => {
      });
      this.dialogRef.close(this.transactionCategoryForm.value);
      this.transactionCategoryForm.reset();
    }
  }

  onCancel() {
    this.dialogRef.close();
    this.transactionCategoryForm.reset();
  }
}
