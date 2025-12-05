import { computed, inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { AuthUser } from './models/auth-user.model';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { LoginRequest } from './models/login.request';
import { RegisterRequest } from './models/register.request';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  handler = inject(HttpBackend);
  http = new HttpClient(this.handler);
  baseApiUrl = 'https://localhost:5000/api/auth';
  authUser = signal<AuthUser>({
    username: null,
    authToken: null,
  });

  isUserAuthenticated = computed(() => {
    return !!this.authUser().authToken;
  });

  login = (credentials: LoginRequest) => {
    this.http
      .post<string>(this.baseApiUrl + '/login', credentials, { responseType: 'text' as 'json' })
      .subscribe((token) => {
        this.authUser.set({
          username: credentials.username,
          authToken: token,
        });
        console.log(this.authUser());
      });
  };
  register = (registerRequest: RegisterRequest) => {
    this.http.post(this.baseApiUrl + '/register', registerRequest).subscribe((response) => {
      console.log(response);
    });
  };
}
