import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Search } from '../search/search';
import { Navbar } from '../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-user-layout',
  imports: [RouterOutlet, Navbar],
  templateUrl: './user-layout.html',
  styleUrl: './user-layout.css',
})
export class UserLayout {}
