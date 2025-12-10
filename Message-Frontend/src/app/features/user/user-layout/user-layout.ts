import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Search } from '../search/search';

@Component({
  selector: 'app-user-layout',
  imports: [RouterOutlet, Search],
  templateUrl: './user-layout.html',
  styleUrl: './user-layout.css',
})
export class UserLayout {}
