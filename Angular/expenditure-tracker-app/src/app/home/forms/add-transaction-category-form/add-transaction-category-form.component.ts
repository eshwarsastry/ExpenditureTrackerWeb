import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TransactionCategory, TransactionType } from '../../../shared/interfaces/interfaces';
import { TransactionTypeService } from '../../../shared/services/transaction-type.service';
import { User } from '../../../shared/interfaces/user';
import { TransactionCategoryService } from '../../../shared/services/transaction-category.service';

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
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.transactionCategoryForm = this.fb.group({
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
        user_Id : this.data.userId,
        transactionType_Id: this.transactionCategoryForm.get('transactionType')?.value,
        name: this.transactionCategoryForm.get('categoryName')?.value,
        description: this.transactionCategoryForm.get('categoryDescription')?.value
      };
      this.transactionCategoryService.addCategory(transactionCategory).subscribe((response) => {
      });
      this.dialogRef.close(this.transactionCategoryForm.value);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
