import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ImportCsvPopupComponent } from '../forms/import-csv-popup/import-csv-popup.component';

@Component({
  selector: 'app-expense-layout',
  templateUrl: './expense-layout.component.html',
  styleUrl: './expense-layout.component.css'
})
export class ExpenseLayoutComponent {
  constructor(private dialog: MatDialog) {}

  openImportCsvDialog(): void {
    console.log('openImportCsvDialog');
    this.dialog.open(ImportCsvPopupComponent, {
      width: '400px',
      disableClose: false
    });
  }
}
