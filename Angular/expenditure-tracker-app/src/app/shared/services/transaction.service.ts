import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Transactions } from '../interfaces/interfaces';

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

  addTransaction(transaction: Transactions): Observable<Transactions> {
    const url = `${this.apiUrl}/ExpenditureLedger/AddLedgerEntry`;

    return this.http.post<Transactions>(url, transaction);
  }

  deleteTransaction(transactionId: number): Observable<void> {
    const url = `${this.apiUrl}/ExpenditureLedger/DeleteTransaction?transactionId=${transactionId}`;

    return this.http.delete<void>(url);
  }
}
