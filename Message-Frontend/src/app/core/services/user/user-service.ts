import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../../models/user';
import { get } from 'http';
import { ChangePasswordRequest } from './DTO/changePasswordRequest';

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

  getUser(id: number) {
    return this.http.get<User>(`${this.userApiUrl}/${id}`);
  }

  changeEmail(email: string) {
    return this.http.put(`${this.userApiUrl}/change-email`, email);
  }

  changePassword(req: ChangePasswordRequest) {
    return this.http.put(`${this.userApiUrl}/change-password`, req);
  }
}
