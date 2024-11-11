import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, of, switchMap, tap } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LOCALSTORAGE_TOKEN_KEY } from '../../app.module';
import { User } from '../interfaces/user';
import { LoginResponse, RegisterRequest, RegisterResponse } from '../interfaces/interfaces';
import { environment } from '../../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

export const fakeLoginResponse: LoginResponse = {
  // fakeAccessToken.....should all come from real backend
  accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c',
  refreshToken: {
    id: 1,
    userId: 2,
    token: 'fakeRefreshToken...should al come from real backend',
    refreshCount: 2,
    expiryDate: new Date(),
  },
  tokenType: 'JWT',
  userId: 1,
  userName: "",
  responseCode: 100,
  message: "Test"
}

export const fakeRegisterResponse: RegisterResponse = {
  responseCode: 200,
  message: 'Registration sucessfull.'
}


@Injectable({
  providedIn: 'root'
})
export class AuthenticateService {

  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private jwtService: JwtHelperService) { }

  login(loginRequest: User): Observable<LoginResponse> {
    const url = `${this.apiUrl}/Login/LoginUser`;

    return this.http.post<LoginResponse>(url, loginRequest);
  }

  register(registerRequest: User): Observable<RegisterResponse> {
    const url = `${this.apiUrl}/login/createuser`;

    return this.http.post<RegisterResponse>(url, registerRequest);
  }

  
   //Get the user from the token payload
   
  getLoggedInUser() {
    const decodedToken = this.jwtService.decodeToken();
    return decodedToken.user;
  }
}
