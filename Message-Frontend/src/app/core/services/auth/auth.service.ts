import { computed, inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { AuthUser } from './models/auth-user.model';
import { HttpClient } from '@angular/common/http';
import { LoginRequest } from './models/login.request';
import { RegisterRequest } from './models/register.request';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost:5000/api/auth';
  authUser = signal<AuthUser>({
    username: null,
    authToken: null,
  });

  isUserAuthenticated = computed<Boolean>(() => {
    return this.authUser().authToken ? true : false;
  });

  login = (credentials: LoginRequest) => {
    this.http
      .post<string>(this.baseApiUrl + '/login', credentials, { responseType: 'text' as 'json' })
      .subscribe((token) => {
        this.authUser().username = credentials.username;
        this.authUser().authToken = token;
      });
  };
  register = (registerRequest: RegisterRequest) => {
    this.http.post(this.baseApiUrl + '/register', registerRequest).subscribe((response) => {
      console.log(response);
    });
  };
}
