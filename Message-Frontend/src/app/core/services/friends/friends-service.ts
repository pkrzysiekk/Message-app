import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { FriendsInvitation } from '../../DTO/friendsInvitation';
import { FriendsInvitationStatus } from '../../DTO/FriendsInvitationStatus';

@Injectable({
  providedIn: 'root',
})
export class FriendsService {
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost/api/friends';

  getFriends() {
    return this.http.get<FriendsInvitation[]>(this.baseApiUrl + '/friends');
  }

  sendInvite(friendId: number) {
    return this.http.post(this.baseApiUrl, friendId);
  }

  getPendingInvites() {
    return this.http.get<FriendsInvitation[]>(this.baseApiUrl + '/invites');
  }

  acceptInvite(friendId: number) {
    return this.http.put(this.baseApiUrl + '/acceptInvite' + `?friendId=${friendId}`, {});
  }

  declineInvite(friendId: number) {
    return this.http.put(this.baseApiUrl + '/declineInvite' + `?friendId=${friendId}`, {});
  }

  removeFriend(friendId: number) {
    return this.http.delete(this.baseApiUrl + '/deleteFriend' + `?friendId=${friendId}`);
  }

  getFriendsStatus(friendId: number) {
    return this.http.get<FriendsInvitationStatus>(
      this.baseApiUrl + '/getFriendsStatus' + `?friendId=${friendId}`,
    );
  }
}
