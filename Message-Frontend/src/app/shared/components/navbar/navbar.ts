import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserService } from '../../../core/services/user/user-service';
import { AuthService } from '../../../core/services/auth/auth.service';
import { User } from '../../../core/models/user';
import { ImageParsePipe } from '../../pipes/image-parse-pipe/image-parse-pipe';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, ImageParsePipe],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  userService = inject(UserService);
  authService = inject(AuthService);

  user = signal<User | null>(null);
  constructor() {
    const userId = this.authService.authUser().userId;
    this.userService.getUser(userId!).subscribe({
      next: (fetchedUser) => {
        this.user.set(fetchedUser);
      },
    });
  }
}
