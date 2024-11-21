import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, map, Observable, of, switchMap, tap } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LOCALSTORAGE_TOKEN_KEY } from '../../app.module';
import { User } from '../interfaces/user';
import { LoginResponse, RegisterRequest, RegisterResponse } from '../interfaces/interfaces';
import { environment } from '../../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SharedService } from './shared.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticateService {

  private apiUrl = environment.apiUrl;
 
  constructor(
    private http: HttpClient,
    private sharedService: SharedService) { }

  login(loginRequest: User): Observable<LoginResponse> {
    const url = `${this.apiUrl}/Login/LoginUser`;

    return this.http.post<LoginResponse>(url, loginRequest);
  }

  register(registerRequest: User): Observable<RegisterResponse> {
    const url = `${this.apiUrl}/login/createuser`;

    return this.http.post<RegisterResponse>(url, registerRequest);
  }
 
  logout() {
    this.sharedService.logout();
  }
}
