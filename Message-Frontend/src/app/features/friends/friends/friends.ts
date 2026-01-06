import { Component, computed, effect, inject, signal } from '@angular/core';
import { FriendsService } from '../../../core/services/friends/friends-service';
import { FriendsInvitation } from '../../../core/DTO/friendsInvitation';
import { error } from 'console';
import { UserService } from '../../../core/services/user/user-service';
import { User } from '../../../core/models/user';
import { forkJoin } from 'rxjs';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-friends',
  imports: [ImageParsePipe, RouterLink],
  templateUrl: './friends.html',
  styleUrl: './friends.css',
})
export class Friends {
  private friendsService = inject(FriendsService);
  private userService = inject(UserService);
  userId = this.userService.localUser()?.id.toString();
  private inviterIds = signal<string[]>([]);
  friends = signal<User[]>([]);
  inviters = signal<User[]>([]);
  showInvites = signal<boolean>(true);

  fetchFriends() {
    this.friendsService.getUsersFromFriends().subscribe({
      next: (fetched) => {
        console.log(fetched);
        this.friends.set(fetched);
      },
    });
  }

  fetchInvites() {
    this.friendsService.getPendingInvites().subscribe({
      next: (invites) => {
        const ids = invites.map((invite) =>
          invite.userId === this.userId ? invite.friendId : invite.userId!,
        );
        console.log(ids);
        this.inviterIds.set(ids);
        console.log('ids', ids);
      },
    });
  }

  constructor() {
    this.fetchFriends();
    this.fetchInvites();

    effect(() => {
      this.inviterIds().map((inv) => {
        this.userService.getUser(parseInt(inv)).subscribe((user) => {
          this.inviters.update((list) => {
            return [...list, user];
          });
        });
      });
    });
  }
  onAccept(friendId: number) {
    this.friendsService.acceptInvite(friendId).subscribe(() => {
      this.fetchInvites();
      this.fetchFriends();
    });
  }
  onReject(friendId: number) {
    this.friendsService.declineInvite(friendId).subscribe(() => {
      this.fetchInvites();
      this.fetchFriends();
    });
  }
  onRemove(friendId: number) {
    this.friendsService.removeFriend(friendId).subscribe({
      next: () => {
        this.fetchFriends();
      },
    });
  }

  onInviteListCollapse() {
    this.showInvites.set(!this.showInvites());
  }
}
