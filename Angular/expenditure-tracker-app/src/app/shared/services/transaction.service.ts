import { HttpClient } from '@angular/common/http';
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

    return this.http.post<Transactions[]>(url, userId);
  }

}
