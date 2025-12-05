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
  baseApiUrl = 'https://localhost/api/auth';
  authUser = signal<AuthUser>({
    username: null,
    authToken: null,
  });

  isUserAuthenticated = computed(() => {
    return !!this.authUser().authToken;
  });

  login = (credentials: LoginRequest) => {
    this.http
      .post<string>(this.baseApiUrl + '/login', credentials, { withCredentials: true })
      .subscribe((token) => {
        console.log('done');
        this.getUser(3);
      });
  };
  register = (registerRequest: RegisterRequest) => {
    this.http.post(this.baseApiUrl + '/register', registerRequest).subscribe((response) => {
      console.log(response);
    });
  };
  getUser(id: number) {
    this.http
      .get('https://localhost/api' + `/user/${id}`, { withCredentials: true })
      .subscribe((response) => console.log(response));
  }
}
