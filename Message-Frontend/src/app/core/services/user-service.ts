import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  userApiUrl = 'https://localhost/api/user';
  http = inject(HttpClient);
  getUser(id: string) {
    return this.http.get<User>(`${this.userApiUrl}/${id}`);
  }
}
