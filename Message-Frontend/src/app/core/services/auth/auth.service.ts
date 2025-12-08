import { computed, inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { AuthUser } from './models/auth-user.model';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { LoginRequest } from './models/login.request';
import { RegisterRequest } from './models/register.request';
import * as signalR from '@microsoft/signalr';
import { Router } from '@angular/router';
import { error } from 'console';

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
  authErr = signal<Error | null>(null);
  router = inject(Router);
  isUserAuthenticated = signal<boolean>(false);

  login = (credentials: LoginRequest) => {
    this.http
      .post<string>(this.baseApiUrl + '/login', credentials, { withCredentials: true })
      .subscribe({
        next: () => {
          this.authUser.set({ username: credentials.username });
        },
        error: (err) => {
          this.authErr.set(err);
        },
      });
  };
  register = (registerRequest: RegisterRequest) => {
    return this.http.post(this.baseApiUrl + '/register', registerRequest).subscribe((response) => {
      console.log(response);
      this.router.navigate(['/auth/register-success']);
    });
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
