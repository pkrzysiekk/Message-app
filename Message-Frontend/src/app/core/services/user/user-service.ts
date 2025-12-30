import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../../models/user';
import { get } from 'http';
import { ChangePasswordRequest } from './DTO/changePasswordRequest';
import { ChangeEmailRequest } from './DTO/changeEmailRequest';
import { Image } from '../../models/image';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  userApiUrl = 'https://localhost/api/user';
  http = inject(HttpClient);
  localUser = signal<User | null>(null);

  private getLocalUserData = () => {
    this.http.get<User>(`${this.userApiUrl}/me`).subscribe({
      next: (user) => {
        this.localUser.set(user);
      },
      error: () => this.localUser.set(null),
    });
  };

  setLocalUser() {
    if (this.localUser()) return;
    this.getLocalUserData();
  }

  clearLocalUser() {
    this.localUser.set(null);
  }

  refreshLocalUser() {
    this.getLocalUserData();
  }

  getUser(id: number) {
    return this.http.get<User>(`${this.userApiUrl}/${id}`);
  }

  changeEmail(req: ChangeEmailRequest) {
    return this.http.put(`${this.userApiUrl}/change-email`, req);
  }

  changePassword(req: ChangePasswordRequest) {
    return this.http.put(`${this.userApiUrl}/change-password`, req);
  }

  changeAvatar(content: File) {
    const formData = new FormData();
    formData.append('avatar', content);
    return this.http.put(`${this.userApiUrl}/change-avatar`, formData);
  }
}
