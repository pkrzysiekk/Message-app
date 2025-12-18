import { Component, inject, signal } from '@angular/core';
import { UserService } from '../../../core/services/user/user-service';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { form, Field, required, email } from '@angular/forms/signals';
import { ChangePasswordRequest } from '../../../core/services/user/DTO/changePasswordRequest';
import { error } from 'node:console';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
@Component({
  selector: 'app-profile',
  imports: [ImageParsePipe, Field],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  userService = inject(UserService);
  showPasswordChange = signal<boolean>(false);
  showEmailChange = signal<boolean>(false);
  userErrorResult = signal<string | null>(null);
  router = inject(Router);
  snackBar = inject(MatSnackBar);

  changeEmailModel = signal({
    email: '',
  });

  changePasswordModel = signal<ChangePasswordRequest>({
    oldPassword: '',
    newPassword: '',
  });

  requiredErrorMessage = 'Required';
  wrongEmailFormatMessage = 'Provide a correct e-mail';

  emailChangedSuccessfullyMessage = 'E-mail changed successfully!';
  passwordChangedSuccessfullyMessage = 'Password changed successfully!';

  changePasswordForm = form(this.changePasswordModel, (schema) => {
    required(schema.oldPassword, { message: this.requiredErrorMessage });
    required(schema.newPassword, { message: this.requiredErrorMessage });
  });
  changeEmailForm = form(this.changeEmailModel, (schema) => {
    required(schema.email, { message: this.requiredErrorMessage });
    email(schema.email, { message: this.wrongEmailFormatMessage });
  });

  showPasswordForm() {
    this.showPasswordChange.set(true);
  }

  showEmailForm() {
    this.showEmailChange.set(true);
    console.log('set');
  }

  onPasswordChange() {
    if (this.changePasswordForm().invalid()) return;
    this.userService
      .changePassword({
        oldPassword: this.changePasswordModel().oldPassword,
        newPassword: this.changePasswordModel().newPassword,
      })
      .subscribe({
        next: () => {
          this.userErrorResult.set(null);
          this.showPasswordChange.set(false);
          this.snackBar.open(this.passwordChangedSuccessfullyMessage, 'Close', { duration: 5000 });
        },
        error: (err: HttpErrorResponse) => {
          this.userErrorResult.set(err.error);
          this.snackBar.open(err.error.error, 'Close', { duration: 5000 });
        },
      });
  }

  onEmailChange() {
    if (this.changeEmailForm().invalid()) return;

    this.userService.changeEmail({ email: this.changeEmailModel().email }).subscribe({
      next: () => {
        this.snackBar.open(this.emailChangedSuccessfullyMessage, 'Close', { duration: 5000 });
        this.userErrorResult.set(null);
        this.showEmailChange.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.snackBar.open('An error has occurred');
      },
    });
  }
  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];

    this.userService.changeAvatar(file).subscribe({
      next: () => {
        this.userService.setLocalUser();
      },
    });
  }
}
