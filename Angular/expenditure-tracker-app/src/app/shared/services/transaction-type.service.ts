import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TransactionType } from '../interfaces/interfaces';

@Injectable({
  providedIn: 'root'
})
export class TransactionTypeService {
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient) { }

  getAllTransactionTypes(): Observable<TransactionType[]> {
    const url = `${this.apiUrl}/ExpenditureLedger/GetAllTransactionTypes`;

    return this.http.get<TransactionType[]>(url);
  }
}
