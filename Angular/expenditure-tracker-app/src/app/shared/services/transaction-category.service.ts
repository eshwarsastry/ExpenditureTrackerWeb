import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TransactionCategory } from '../interfaces/interfaces';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TransactionCategoryService {

  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient) { }

  getAllTransactionCategories(userId: number): Observable<TransactionCategory[]> {
    const url = `${this.apiUrl}/ExpenditureLedger/GetAllCategoriesOfUser`;
    const params = new HttpParams().set('userId', userId.toString());

    return this.http.get<TransactionCategory[]>(url, { params });
  }

  addCategory(category: TransactionCategory): Observable<TransactionCategory> {
    const url = `${this.apiUrl}/ExpenditureLedger/AddCategory`;

    return this.http.post<TransactionCategory>(url, category);
  }
}
