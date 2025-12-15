import { Component, inject, signal } from '@angular/core';
import { UserService } from '../../../core/services/user/user-service';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { form, Field } from '@angular/forms/signals';
import { ChangePasswordRequest } from '../../../core/services/user/DTO/changePasswordRequest';
import { error } from 'node:console';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
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
  snackBar = inject(MatSnackBar);

  changeEmailModel = signal({
    email: '',
  });

  changePasswordModel = signal<ChangePasswordRequest>({
    oldPassword: '',
    newPassword: '',
  });

  changePasswordForm = form(this.changePasswordModel);
  changeEmailForm = form(this.changeEmailModel);

  showPasswordForm() {
    this.showPasswordChange.set(true);
  }

  showEmailForm() {
    this.showEmailChange.set(true);
    console.log('set');
  }

  onPasswordChange() {
    this.userService
      .changePassword({
        oldPassword: this.changePasswordModel().oldPassword,
        newPassword: this.changePasswordModel().newPassword,
      })
      .subscribe({
        next: () => {
          this.userErrorResult.set(null);
        },
        error: (err: HttpErrorResponse) => {
          this.userErrorResult.set(err.error);
        },
      });
  }

  onEmailChange() {
    this.userService.changeEmail({ email: this.changeEmailModel().email }).subscribe({
      next: () => {
        this.snackBar.open('E-mail changed successfully!', 'Close', { duration: 5000 });
        this.userErrorResult.set(null);
        this.showEmailChange.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.snackBar.open('An error has occurred');
      },
    });
  }
}
