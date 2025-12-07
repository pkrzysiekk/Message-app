import { Component, signal, Signal } from '@angular/core';
import { Search } from '../search/search';
import { User } from '../../../core/models/user';

@Component({
  selector: 'app-home',
  imports: [Search],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {}
