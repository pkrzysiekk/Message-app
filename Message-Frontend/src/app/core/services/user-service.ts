import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  userApiUrl = 'https://localhost/api/user';
  http = inject(HttpClient);
  getUser(id: number) {
    return this.http.get(`${this.userApiUrl}/${id}`);
  }
}
