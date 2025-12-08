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
  });
  authErr = signal<HttpErrorResponse | null>(null);
  router = inject(Router);
  isUserAuthenticated = signal<boolean>(false);

  login = (credentials: LoginRequest) => {
    return this.http.post<string>(this.baseApiUrl + '/login', credentials).pipe(
      tap(() => {
        this.authUser.set({ username: credentials.username });
        this.router.navigate(['/app']);
      }),
    );
  };

  register = (registerRequest: RegisterRequest) => {
    return this.http.post(this.baseApiUrl + '/register', registerRequest);
  };

  getUser(id: number) {
    this.http
      .get('https://localhost/api' + `/user/${id}`, { withCredentials: true })
      .subscribe((response) => console.log(response));
  }
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
