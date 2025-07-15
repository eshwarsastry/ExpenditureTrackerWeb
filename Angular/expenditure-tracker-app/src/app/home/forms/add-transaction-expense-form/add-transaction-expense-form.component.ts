import { ChangeDetectionStrategy, Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { provideNativeDateAdapter } from '@angular/material/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { TransactionTypeService } from '../../../shared/services/transaction-type.service';
import { TransactionCategoryService } from '../../../shared/services/transaction-category.service';
import { TransactionCategory, TransactionType, Transactions } from '../../../shared/interfaces/interfaces';
import { TransactionService } from '../../../shared/services/transaction.service';
import { SharedService } from '../../../shared/services/shared.service';
import { ImportDataService } from '../../../shared/services/import-data.service';

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
  displayedtransactionCategories: TransactionCategory[] = [];
  showManualForm: boolean = false;
  isUploading: boolean = false;

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddTransactionExpenseFormComponent>,
    private dialog: MatDialog,
    private transactionTypeService: TransactionTypeService,
    private transactionCategoryService: TransactionCategoryService,
    private transactionService: TransactionService,
    private sharedService: SharedService,
    @Inject(MAT_DIALOG_DATA) public dailogData: any,
    private importDataService: ImportDataService,
    private cdRef: ChangeDetectorRef) {
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
    this.transactionCategoryService.getAllTransactionCategories(this.dailogData.loggedInUserId).subscribe((response) => {
      this.transactionCategories = response;
      if (this.dailogData.editable) {
        this.updateTransactioncategories(this.dailogData.transactionRow.transactionType_Id);
        this.setFormValues();
      }
    });

    // Observe changes on the transaction type control
    this.transactionForm.get('transactionType')?.valueChanges.subscribe(selectedTypeId => {
      this.updateTransactioncategories(selectedTypeId);
    });

    
  }

  setFormValues() {
    this.transactionForm.patchValue({
      id: this.dailogData.transactionRow.id ?? 0,
      transactionType: this.dailogData.transactionRow.transactionType_Id,
      transactionCategory: this.dailogData.transactionRow.category_Id,
      transactionDate: this.dailogData.transactionRow.transactionDate,
      transactionAmount: this.dailogData.transactionRow.amount,
      transactionNote: this.dailogData.transactionRow.note,
    })
  }

  // Update categories based on selected transaction type
  updateTransactioncategories(typeId: number) {
    this.displayedtransactionCategories = this.transactionCategories.filter(subcat => subcat.transactionType_Id === typeId);
    // Clear category selection if type changes
    this.transactionForm.get('transactionCategory')?.setValue(null);
  }

  onSave(): void {
    if (this.transactionForm.valid) {
      const transaction: Transactions = {
        id: this.dailogData.transactionRow.id ?? 0,
        user_Id: this.dailogData.loggedInUserId,
        category_Id: this.transactionForm.get('transactionCategory')?.value,
        category_Name: '',
        transactionType_Id: this.transactionForm.get('transactionType')?.value,
        transactionType_Name: '',
        amount: this.transactionForm.get('transactionAmount')?.value,
        transactionDate: this.transactionForm.get('transactionDate')?.value,
        note: this.transactionForm.get('transactionNote')?.value
      };
      this.transactionService.addTransaction(transaction).subscribe((response) => {
      });
      this.dialogRef.close(this.transactionForm.value);
      this.transactionForm.reset();
    }
  }

  onCancel() {
    this.dialogRef.close();
    this.transactionForm.reset();
  }

  openAddCategoryDialog() {
    this.sharedService.openAddTransactionCategoryDialog(this.dailogData.loggedInUserId, false, {
      id: 0, user_Id: 0, transactionType_Id: 0, transactionType_Name: '', name: '', description: ''
    }).subscribe((result: any) => {
      if (result && result.categoryName) { // or result.id if you return the whole object
        this.transactionCategoryService.getAllTransactionCategories(this.dailogData.loggedInUserId).subscribe((response) => {
          this.transactionCategories = response;
          const currentTypeId = this.transactionForm.get('transactionType')?.value;
          if (currentTypeId) {
            this.updateTransactioncategories(currentTypeId);
          }
          // Find the new category by name (or by id if available)
          const newCategory = this.transactionCategories.find(cat =>
            cat.name === result.categoryName && cat.transactionType_Id === currentTypeId
          );
          if (newCategory) {
            this.transactionForm.get('transactionCategory')?.setValue(newCategory.id);
          }
        });
      }
    });
  }

  enableManualEntry() {
    this.showManualForm = true;
  }

  triggerFileInput(fileInput: HTMLInputElement) {
    fileInput.click();
  }

  onBillImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.isUploading = true;
      this.importDataService.uploadBillImage(file, this.dailogData.loggedInUserId).subscribe({
        next: (response) => {
          try {
            this.transactionForm.patchValue({
              transactionType: response.transactionType_Id,
              transactionCategory: response.category_Id,
              transactionDate: response.transactionDate,
              transactionAmount: response.amount,
              transactionNote: response.note
            });
          } catch (e) {
            console.error('Error patching form:', e);
          }
          this.showManualForm = true;
          this.isUploading = false;
          this.cdRef.markForCheck();
          console.log('Upload success, isUploading:', this.isUploading, 'showManualForm:', this.showManualForm);
        },
        error: (err) => {
          this.transactionForm.reset();
          this.showManualForm = true;
          this.isUploading = false;
          this.cdRef.markForCheck();
          console.log('Upload error, isUploading:', this.isUploading, 'showManualForm:', this.showManualForm);
        }
      });
    }
  }
}
