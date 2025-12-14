import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from '../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-profile-layout',
  imports: [RouterOutlet, Navbar],
  templateUrl: './profile-layout.html',
  styleUrl: './profile-layout.css',
})
export class ProfileLayout {}
