import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from '../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-friends-layout',
  imports: [RouterOutlet, Navbar],
  templateUrl: './friends-layout.html',
  styleUrl: './friends-layout.css',
})
export class FriendsLayout {}
