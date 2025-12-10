import { Component, signal, Signal } from '@angular/core';
import { Search } from '../../user/search/search';

@Component({
  selector: 'app-home',
  imports: [Search],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {}
