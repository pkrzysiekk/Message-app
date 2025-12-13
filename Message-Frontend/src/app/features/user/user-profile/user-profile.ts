import { Component, effect, inject, signal } from '@angular/core';
import { UserService } from '../../../core/services/user-service';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { User } from '../../../core/models/user';
import { error } from 'console';
import { finalize } from 'rxjs';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { FriendsService } from '../../../core/services/friends/friends-service';
import { FriendsInvitation } from '../../../core/DTO/friendsInvitation';
import { FriendsInvitationStatus } from '../../../core/DTO/FriendsInvitationStatus';

@Component({
  selector: 'app-user-profile',
  imports: [ImageParsePipe],
  templateUrl: './user-profile.html',
  styleUrl: './user-profile.css',
})
export class UserProfile {
  userService = inject(UserService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  protected friendsService = inject(FriendsService);
  FriendsInvStatus = FriendsInvitationStatus;
  fetchedUser = signal<User | null>(null);
  readonly userId = signal<number | null>(null);
  private isLoading = signal<boolean>(false);
  protected friendsStatus = signal<FriendsInvitationStatus | null>(null);
  protected isInvited = signal<boolean | null>(null);

  loadData = () => {
    this.isLoading.set(true);
    const id = this.userId();
    if (!id) {
      this.router.navigate(['/app']);
      return;
    }

    this.userService
      .getUser(id)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (fetched) => {
          this.fetchedUser.set(fetched);
          console.log(fetched);
        },
        error: (e) => console.log(e),
      });
  };

  constructor() {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');

      this.isInvited.set(null);
      this.userId.set(parseFloat(id!));
    });

    effect(() => {
      const id = this.userId();
      if (id) {
        this.loadData();
        this.loadFriendsStatus();
        console.log(this.isInvited());
      }
    });
  }
  loadFriendsStatus = () => {
    this.friendsService.getFriendsStatus(this.userId()!).subscribe({
      next: (result) => {
        console.log(result);
        this.friendsStatus.set(result);
      },
    });
  };
  onInvite = () => {
    this.friendsService.sendInvite(this.userId()!).subscribe({
      next: () => {
        this.isInvited.set(true);
      },
    });
  };

  onRemove = () => {
    this.friendsService.removeFriend(this.userId()!).subscribe({
      next: () => {
        this.isInvited.set(false);
      },
    });
  };
}
