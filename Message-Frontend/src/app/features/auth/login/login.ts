import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LoginModel } from '../models/login.form.model';
import { form, Field, required } from '@angular/forms/signals';
import { AuthService } from '../../../core/services/auth/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
  imports: [RouterLink, Field],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  authService = inject(AuthService);
  loginModel = signal<LoginModel>({
    username: '',
    password: '',
  });

  loginError = signal<HttpErrorResponse | null>(null);

  loginForm = form(this.loginModel, (schemaPath) => {
    required(schemaPath.username, { message: 'Username is required' });
    required(schemaPath.password, { message: 'Password is required' });
  });

  handleLogin = () => {
    this.authService
      .login({
        username: this.loginModel().username,
        password: this.loginModel().password,
      })
      .subscribe({
        next: () => {},
        error: (e: HttpErrorResponse) => this.loginError.set(e.error),
      });
  };
}
