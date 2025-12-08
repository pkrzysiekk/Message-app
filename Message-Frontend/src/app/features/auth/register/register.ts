import { Component, inject, signal } from '@angular/core';

import { RouterLink } from '@angular/router';
import { RegisterModel } from '../models/register.form.model';
import {
  email,
  form,
  minLength,
  required,
  SchemaPath,
  validate,
  Field,
  maxLength,
} from '@angular/forms/signals';
import { equal } from 'assert';
import { error } from 'console';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth/auth.service';
import { finalize } from 'rxjs';
@Component({
  selector: 'app-register',
  imports: [RouterLink, Field],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  authService = inject(AuthService);

  registerModel = signal<RegisterModel>({
    username: '',
    password: '',
    confirmPassword: '',
    email: '',
  });

  registerError = signal<HttpErrorResponse | null>(null);

  isLoading = signal<boolean>(false);

  requiredUsernameLength = 6;
  requiredPasswordLength = 6;

  maxUsernameLength = 30;
  maxPasswordLength = 40;

  requiredFieldErrorMessage = 'This field is required';
  passwordsDoesNotMatchErrorMessage = 'Passwords does not match';
  wrongEmailFormatErrorMessage = 'Wrong Email format';
  usernameLengthTooShortErrorMessage = `Field must have at least ${this.requiredUsernameLength} letters`;
  passwordLengthTooShortErrorMessage = `Field must have at least ${this.requiredPasswordLength} letters`;

  usernameTooLongErrorMessage = `Username can have ${this.maxUsernameLength} characters max`;
  passwordTooLongErrorMessage = `Password can have ${this.maxPasswordLength} characters max`;

  registerForm = form(this.registerModel, (schemaPath) => {
    required(schemaPath.username, { message: this.requiredFieldErrorMessage });
    required(schemaPath.password, { message: this.requiredFieldErrorMessage });
    required(schemaPath.confirmPassword, { message: this.requiredFieldErrorMessage });
    required(schemaPath.email, { message: this.requiredFieldErrorMessage });
    email(schemaPath.email, { message: this.wrongEmailFormatErrorMessage });
    minLength(schemaPath.username, this.requiredUsernameLength, {
      message: this.usernameTooLongErrorMessage,
    });
    minLength(schemaPath.password, this.requiredPasswordLength, {
      message: this.passwordLengthTooShortErrorMessage,
    });
    maxLength(schemaPath.username, this.maxUsernameLength, {
      message: this.usernameTooLongErrorMessage,
    });
    maxLength(schemaPath.password, this.maxPasswordLength, {
      message: this.passwordTooLongErrorMessage,
    });
    //check if passwords match
    validate(schemaPath.confirmPassword, ({ value, valueOf }) => {
      const password = valueOf(schemaPath.password);
      const confirmPassword = valueOf(schemaPath.confirmPassword);
      if (!password || !confirmPassword) return null;
      if (password != confirmPassword) {
        return {
          kind: 'Passwords mismatch',
          message: this.passwordsDoesNotMatchErrorMessage,
        };
      }
      return null;
    });
  });

  onRegister = () => {
    if (this.registerForm().invalid()) return;
    this.isLoading.set(true);
    this.authService
      .register({
        username: this.registerModel().username,
        password: this.registerModel().password,
        email: this.registerModel().email,
      })
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({ error: (e: HttpErrorResponse) => this.registerError.set(e.error) });
  };
}
