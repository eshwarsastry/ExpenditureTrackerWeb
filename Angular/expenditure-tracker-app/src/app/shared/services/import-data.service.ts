import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ImportDataService {
  private apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) {

   }
   
   importData(file: File, userId: number): Observable<any> {
    const formData = new FormData();
    formData.append('importFile', file);
    formData.append('userId', userId.toString());
    const url = `${this.apiUrl}/ExpenditureLedger/ImportDataFromCSV`;

    return this.http.post<any>(url, formData);
  }

  uploadBillImage(file: File, userId: number): Observable<any> {
    const formData = new FormData();
    formData.append('importFile', file);
    formData.append('userId', userId.toString());
    const url = `${this.apiUrl}/ExpenditureLedger/UploadExpenseDetailsFromBill`;

    return this.http.post<any>(url, formData);
  }
}
