import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

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

  constructor(
    private http: HttpClient,
    private dialogRef: MatDialogRef<ImportCsvPopupComponent>
  ) {}

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
    const formData = new FormData();
    formData.append('file', this.selectedFile);
    this.errorMessage = '';
    this.successMessage = '';
    this.http.post('/api/import-csv', formData).subscribe({
      next: () => {
        this.successMessage = 'CSV imported successfully!';
        setTimeout(() => this.dialogRef.close(true), 1200);
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage = err.error?.message || 'Failed to import CSV.';
      }
    });
  }
}
