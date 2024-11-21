import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  // Subjects to store userId values
  private userIdKey = 'userId';

  public userIdSubject = new BehaviorSubject<number>(this.getUserIdFromStorage());

  constructor() { }

  private dataSubject = new BehaviorSubject<any>(null);
  data$ = this.dataSubject.asObservable();

  sendData(data: any) {
    this.dataSubject.next(data); // Send data to all subscribers
  }

  private getUserIdFromStorage(): number {
    return Number(localStorage.getItem(this.userIdKey));
  }

  public setUserId(userId: number) {
    localStorage.setItem(this.userIdKey, userId.toString());
    this.userIdSubject.next(userId);
  }

  getUserId(): Observable<number> {
    return this.userIdSubject.asObservable();
  }

  // Logout method that clears storage and emits null values
  logout() {
    localStorage.removeItem(this.userIdKey);
    this.userIdSubject.next(0);
  }
}

