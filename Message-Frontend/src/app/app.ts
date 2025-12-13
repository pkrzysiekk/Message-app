import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UserService } from './core/services/user/user-service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Message-Frontend');
  protected readonly userService = inject(UserService);
  ngOnInit() {
    this.userService.setLocalUser();
  }
}
