import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { SharedService } from '../../../shared/services/shared.service';
import { ImportDataService } from '../../../shared/services/import-data.service';

@Component({
  selector: 'app-import-csv-popup',
  templateUrl: './import-csv-popup.component.html',
  styleUrl: './import-csv-popup.component.css'
})
export class ImportCsvPopupComponent {
  selectedFile: File | null = null;
  fileName: string = '';
  errorMessage: string = '';
  successMessage: string = '';
  loggedInUserId: number = 0;
  isLoading: boolean = false;
  constructor(
    private dialogRef: MatDialogRef<ImportCsvPopupComponent>,
    private importDataService: ImportDataService,
    private sharedService: SharedService,
  ) {}

  ngOnInit() {
    this.sharedService.userIdSubject.subscribe((data) => {
      this.loggedInUserId = data;
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.fileName = this.selectedFile.name;
      this.errorMessage = '';
      this.successMessage = '';
    }
  }

  onUpload(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a CSV file.';
      return;
    }
    this.isLoading = true;

    this.importDataService.importData(this.selectedFile, this.loggedInUserId).subscribe({
      next: (response) => {
        this.successMessage = response.message;
        this.isLoading = false;
        setTimeout(() => this.dialogRef.close(true), 1200);
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage = err.error?.message || 'Failed to import CSV.';
        this.isLoading = false;
      }
    });
  }
}
