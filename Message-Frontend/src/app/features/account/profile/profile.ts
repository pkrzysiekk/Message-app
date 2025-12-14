import { Component, inject } from '@angular/core';
import { UserService } from '../../../core/services/user/user-service';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';
@Component({
  selector: 'app-profile',
  imports: [ImageParsePipe],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  userService = inject(UserService);
}
