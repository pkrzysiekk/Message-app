import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from '../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-home-layout',
  imports: [RouterOutlet, Navbar],
  templateUrl: './home-layout.html',
  styleUrl: './home-layout.css',
})
export class HomeLayout {}
