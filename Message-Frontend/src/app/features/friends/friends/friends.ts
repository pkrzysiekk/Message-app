import { Component, computed, effect, inject, signal } from '@angular/core';
import { FriendsService } from '../../../core/services/friends/friends-service';
import { FriendsInvitation } from '../../../core/DTO/friendsInvitation';
import { error } from 'console';
import { UserService } from '../../../core/services/user/user-service';
import { User } from '../../../core/models/user';
import { forkJoin } from 'rxjs';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';

@Component({
  selector: 'app-friends',
  imports: [ImageParsePipe],
  templateUrl: './friends.html',
  styleUrl: './friends.css',
})
export class Friends {
  private friendsService = inject(FriendsService);
  private userService = inject(UserService);
  userId = this.userService.localUser()?.id.toString();
  private fetchedInvites = signal<FriendsInvitation[] | null>(null);
  private friendIds = signal<string[]>([]);
  private inviterIds = signal<string[]>([]);
  friends = signal<User[]>([]);
  inviters = signal<User[]>([]);
  showInvites = signal<boolean>(true);

  fetchFriends() {
    this.friendsService.getFriends().subscribe({
      next: (friends) => {
        const ids = friends.map((friend) =>
          friend.userId === this.userId ? friend.friendId : friend.userId!,
        );
        this.friendIds.set(ids);
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
      },
    });
  }

  constructor() {
    this.fetchFriends();
    this.fetchInvites();

    effect((onCleanup) => {
      const ids = this.friendIds();
      console.log(ids);
      if (!ids.length) {
        this.friends.set([]);
        return;
      }
      const sub = forkJoin(ids.map((id) => this.userService.getUser(+id))).subscribe({
        next: (friends) => {
          this.friends.set(friends);
        },
      });

      onCleanup(() => sub.unsubscribe());
    });

    effect((onCleanup) => {
      const ids = this.inviterIds();
      if (!ids.length) this.inviters.set([]);
      const sub = forkJoin(ids.map((id) => this.userService.getUser(+id))).subscribe({
        next: (inv) => {
          this.inviters.set(inv);
          console.log('inv', inv);
        },
      });
      onCleanup(() => sub.unsubscribe());
    });
  }
  onAccept(friendId: number) {
    this.friendsService.acceptInvite(friendId).subscribe(() => {
      this.fetchInvites();
    });
  }
  onReject(friendId: number) {
    this.friendsService.declineInvite(friendId).subscribe(() => {
      this.fetchInvites();
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
