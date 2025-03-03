import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TransactionTableFilter, Transactions } from '../interfaces/interfaces';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient) { }

  getAllTransactions(userId: number): Observable<Transactions[]> {
    const url = `${this.apiUrl}/ExpenditureLedger/GetAllExpensesOfUser`;
    const params = new HttpParams().set('userId', userId.toString());

    return this.http.get<Transactions[]>(url, { params });
  }

  getRecentTransactions(userId: number): Observable<Transactions[]> {
    const url = `${this.apiUrl}/ExpenditureLedger/GetRecentExpensesOfUser`;
    const params = new HttpParams().set('userId', userId.toString());

    return this.http.get<Transactions[]>(url, { params });
  }

  getTransactionsByFilter(filterParams: TransactionTableFilter): Observable<Transactions[]> {
    const url = `${this.apiUrl}/ExpenditureLedger/GetExpensesOfUserByFilter`;
    const params = new HttpParams()
      .set('user_Id', filterParams.user_Id)
      .set('month', filterParams.month)
      .set('year', filterParams.year);
    return this.http.get<Transactions[]>(url, { params });
  }

  addTransaction(transaction: Transactions): Observable<Transactions> {
    const url = `${this.apiUrl}/ExpenditureLedger/AddLedgerEntry`;

    return this.http.post<Transactions>(url, transaction);
  }

  deleteTransaction(transactionId: number): Observable<void> {
    const url = `${this.apiUrl}/ExpenditureLedger/DeleteTransaction?transactionId=${transactionId}`;

    return this.http.delete<void>(url);
  }
}
