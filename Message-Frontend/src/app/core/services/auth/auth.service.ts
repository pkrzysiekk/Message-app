import { computed, inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { AuthUser } from './models/auth-user.model';
import { HttpBackend, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { LoginRequest } from './models/login.request';
import { RegisterRequest } from './models/register.request';
import * as signalR from '@microsoft/signalr';
import { Router } from '@angular/router';
import { error } from 'console';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  handler = inject(HttpBackend);
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost/api/auth';
  authUser = signal<AuthUser>({
    username: null,
    userId: null,
  });
  authErr = signal<HttpErrorResponse | null>(null);
  router = inject(Router);
  isUserAuthenticated = signal<boolean>(false);

  login = (credentials: LoginRequest) => {
    return this.http.post<number>(this.baseApiUrl + '/login', credentials).pipe(
      tap((userId) => {
        console.log(userId);
        this.authUser.set({ username: credentials.username, userId: userId });
        this.router.navigate(['/app']);
      }),
    );
  };

  register = (registerRequest: RegisterRequest) => {
    return this.http
      .post(this.baseApiUrl + '/register', registerRequest)
      .pipe(tap(() => this.router.navigate(['/auth/register-success'])));
  };

  getInvites(id: number) {
    this.http
      .get('https://localhost/api' + `/friends/users/${id}/invites`, { withCredentials: true })
      .subscribe((response) => console.log(response));
  }
  // async chat() {
  //   const connection = new signalR.HubConnectionBuilder()
  //     .withUrl('https://localhost/ChatHub', { withCredentials: true })
  //     .withAutomaticReconnect()
  //     .build();

  //   await connection.start();
  // }
}
