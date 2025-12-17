import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserService } from '../../../core/services/user/user-service';
import { AuthService } from '../../../core/services/auth/auth.service';
import { User } from '../../../core/models/user';
import { ImageParsePipe } from '../../pipes/image-parse-pipe/image-parse-pipe';
import { ClickedOutside } from '../../directives/clicked-outside/clicked-outside';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, ImageParsePipe, ClickedOutside],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  authService = inject(AuthService);
  userService = inject(UserService);
  showProfileContextMenu = signal<boolean>(false);

  onProfileContextMenuClick() {
    this.showProfileContextMenu.set(!this.showProfileContextMenu());
  }
  onHideContextMenu() {
    this.showProfileContextMenu.set(false);
  }

  onLogOut() {
    this.authService.logout().subscribe();
  }
}
