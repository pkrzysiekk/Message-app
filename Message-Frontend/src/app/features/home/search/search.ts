import { HttpClient } from '@angular/common/http';
import { Component, inject, model, signal } from '@angular/core';
import { FormsModule, NgModel } from '@angular/forms';
import { Field, form } from '@angular/forms/signals';
import { debounceTime } from 'rxjs';
import { User } from '../../../core/services/auth/models/user';

@Component({
  selector: 'app-search',
  imports: [FormsModule],
  templateUrl: './search.html',
  styleUrl: './search.css',
})
export class Search {
  http = inject(HttpClient);
  searchTerm = '';
  fetchedUsers = signal<User[]>([]);
  onSearch = () => {
    if (!this.searchTerm) return;
    const reqUrl = `https://localhost/api/user?term=${this.searchTerm}`;
    this.http
      .get<User[]>(reqUrl)
      .pipe(debounceTime(1000))
      .subscribe((resp) => this.fetchedUsers.set(resp));
  };
}
