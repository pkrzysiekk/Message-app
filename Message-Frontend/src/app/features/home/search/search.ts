import { HttpClient } from '@angular/common/http';
import { Component, inject, model, signal } from '@angular/core';
import { FormsModule, NgModel } from '@angular/forms';
import { Field, form } from '@angular/forms/signals';
import { debounceTime } from 'rxjs';
import { User } from '../../../core/models/user';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { ClickedOutside } from '../../../shared/directives/clicked-outside/clicked-outside';

@Component({
  selector: 'app-search',
  imports: [FormsModule, ImageParsePipe, ClickedOutside],
  templateUrl: './search.html',
  styleUrl: './search.css',
})
export class Search {
  http = inject(HttpClient);
  searchTerm = '';
  fetchedUsers = model<User[]>([]);
  deleteFetchedUsers = () => {
    this.fetchedUsers.set([]);
  };
  onSearch = () => {
    if (!this.searchTerm) {
      this.fetchedUsers.set([]);
      return;
    }
    const reqUrl = `https://localhost/api/user?term=${this.searchTerm}`;
    this.http
      .get<User[]>(reqUrl)
      .pipe(debounceTime(1000))
      .subscribe((resp) => {
        this.fetchedUsers.set(resp);
      });
  };
}
