import { Component } from '@angular/core';
import { Navbar } from '../../../shared/components/navbar/navbar';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-group-layout',
  imports: [Navbar, RouterOutlet],
  templateUrl: './group-layout.html',
  styleUrl: './group-layout.css',
})
export class GroupLayout {}
