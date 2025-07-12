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
   
   importData(formData: FormData): Observable<any> {
    const url = `${this.apiUrl}/ExpenditureLedger/ImportDataFromCSV`;

    return this.http.post<any>(url, formData);
  }
}
