import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TransactionCategory } from '../interfaces/interfaces';

@Injectable({
  providedIn: 'root'
})
export class TransactionCategoryService {

  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient) { }

  getAllTransactionCategories(userId: number): Observable<TransactionCategory[]> {
    const url = `${this.apiUrl}/ExpenditureLedger/GetAllCategoriesOfUser`;

    return this.http.post<TransactionCategory[]>(url, userId);
  }

  addCategory(userId: number): Observable<TransactionCategory> {
    const url = `${this.apiUrl}/ExpenditureLedger/GetAllCategoriesOfUser`;

    return this.http.post<TransactionCategory>(url, userId);
  }
}
