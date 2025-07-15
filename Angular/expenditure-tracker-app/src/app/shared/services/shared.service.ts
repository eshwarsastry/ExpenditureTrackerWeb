import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { AddTransactionCategoryFormComponent } from '../../home/forms/add-transaction-category-form/add-transaction-category-form.component';
import { CategoryDialogData, TransactionCategory } from '../interfaces/interfaces';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  // Subjects to store userId values
  private userIdKey = 'userId';

  public userIdSubject = new BehaviorSubject<number>(this.getUserIdFromStorage());

  constructor(private dialog: MatDialog) { }

  private dataSubject = new BehaviorSubject<any>(null);
  data$ = this.dataSubject.asObservable();

  sendData(data: any) {
    this.dataSubject.next(data); // Send data to all subscribers
  }

  private getUserIdFromStorage(): number {
    return Number(localStorage.getItem(this.userIdKey));
  }

  public setUserId(userId: number) {
    localStorage.setItem(this.userIdKey, userId.toString());
    this.userIdSubject.next(userId);
  }

  getUserId(): Observable<number> {
    return this.userIdSubject.asObservable();
  }

  // Logout method that clears storage and emits null values
  logout() {
    localStorage.removeItem(this.userIdKey);
    this.userIdSubject.next(0);
  }

  // Method to open the add transaction category dialog
  openAddTransactionCategoryDialog(loggedInUserId: number, editable: boolean = false, transactionCategoryRow: TransactionCategory = { id: 0, user_Id: 0, transactionType_Id: 0, transactionType_Name: '', name: '', description: '' }) {
    const dialogData: CategoryDialogData = {
      transactionCategoryRow: transactionCategoryRow,
      loggedInUserId: loggedInUserId,
      editable: editable
    };
    
    const dialogRef = this.dialog.open(AddTransactionCategoryFormComponent, {
      width: '400px',
      data: dialogData
    });

    return dialogRef.afterClosed();
  }
}

