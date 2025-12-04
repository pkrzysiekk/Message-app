import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LoginModel } from '../models/login.form.model';
import { form, Field, required } from '@angular/forms/signals';

@Component({
  selector: 'app-login',
  imports: [RouterLink, Field],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginModel = signal<LoginModel>({
    username: '',
    password: '',
  });

  loginForm = form(this.loginModel, (schemaPath) => {
    required(schemaPath.username, { message: 'Username is required' });
    required(schemaPath.password, { message: 'Password is required' });
  });
}
