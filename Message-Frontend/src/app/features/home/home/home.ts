import { Component, signal, Signal } from '@angular/core';
import { Search } from '../../user/search/search';
import { Navbar } from '../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-home',
  imports: [Search, Navbar],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {}
