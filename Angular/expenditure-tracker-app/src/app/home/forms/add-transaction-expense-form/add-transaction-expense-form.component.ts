import { ChangeDetectionStrategy, Component, OnInit, Inject } from '@angular/core';
import { provideNativeDateAdapter } from '@angular/material/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TransactionTypeService } from '../../../shared/services/transaction-type.service';
import { TransactionCategoryService } from '../../../shared/services/transaction-category.service';
import { TransactionCategory, TransactionType, Transactions } from '../../../shared/interfaces/interfaces';

@Component({
  selector: 'app-add-transaction-expense-form',
  templateUrl: './add-transaction-expense-form.component.html',
  styleUrl: './add-transaction-expense-form.component.css',
  providers: [provideNativeDateAdapter()],
  changeDetection: ChangeDetectionStrategy.OnPush,
})

export class AddTransactionExpenseFormComponent {

  transactionForm: FormGroup;
  transactionTypes: TransactionType[] = [];
  transactionCategories: TransactionCategory[] = [];


  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddTransactionExpenseFormComponent>,
    private transactionTypeService: TransactionTypeService,
    private transactionCategoryService: TransactionCategoryService,
    @Inject(MAT_DIALOG_DATA) public dailogData: any) {
    this.transactionForm = this.fb.group({
      id: [''],
      transactionType: ['', Validators.required],
      transactionCategory: ['', Validators.required],
      transactionDate: ['', Validators.required],
      transactionAmount: [null, [Validators.required, Validators.min(0.01)]],
      transactionNote: ['']
    });
  }

  ngOnInit(): void {
    this.transactionTypeService.getAllTransactionTypes().subscribe((response) => {
      this.transactionTypes = response;
    });
    this.transactionCategoryService.getAllTransactionCategories(this.dailogData.loggedInUserData.userId).subscribe((response) => {
      this.transactionCategories = response;
    });

    // Observe changes on the transaction type control
    this.transactionForm.get('transactionType')?.valueChanges.subscribe(selectedTypeId => {
      this.updateTransactioncategories(selectedTypeId);
    });
  }

  // Update categories based on selected transaction type
  updateTransactioncategories(typeId: number) {
    this.transactionCategories = this.transactionCategories.filter(subcat => subcat.transactionType_Id === typeId);
    // Clear category selection if type changes
    this.transactionForm.get('transactionCategory')?.setValue(null);
  }

  onSave(): void {
    if (this.transactionForm.valid) {
      const transaction: Transactions = {
        id: this.dailogData.transactionCategoryRow.id ?? 0,
        user_Id: this.dailogData.loggedInUserData.userId,
        category_Id: this.transactionForm.get('transactionCategory')?.value,
        amount: this.transactionForm.get('transactionAmount')?.value,
        transactionDate: this.transactionForm.get('transactionDate')?.value,
        note: this.transactionForm.get('transactionNote')?.value
      };
      //this.transactionTypeService.addTransaction(transaction).subscribe((response) => {
      //});
      this.dialogRef.close(this.transactionForm.value);
      this.transactionForm.reset();
    }
  }

  onCancel() {
    this.dialogRef.close();
    this.transactionForm.reset();
  }
}
