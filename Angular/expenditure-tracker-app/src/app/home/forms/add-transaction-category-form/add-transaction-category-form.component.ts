import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-add-transaction-category-form',
  templateUrl: './add-transaction-category-form.component.html',
  styleUrl: './add-transaction-category-form.component.css'
})

export class AddTransactionCategoryFormComponent {
  transactionCategoryForm: FormGroup;
  transactionTypes: string[] = ['Income', 'Expenditure'];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddTransactionCategoryFormComponent>
  ) {
    this.transactionCategoryForm = this.fb.group({
      transactionType: ['', Validators.required],
      categoryName: ['', Validators.required]
    });
  }

  onSave() {
    if (this.transactionCategoryForm.valid) {
      this.dialogRef.close(this.transactionCategoryForm.value);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
