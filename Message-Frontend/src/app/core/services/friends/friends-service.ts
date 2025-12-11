import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { FriendsInvitation } from '../../DTO/friendsInvitation';

@Injectable({
  providedIn: 'root',
})
export class FriendsService {
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost/api/friends';

  getFriends() {
    return this.http.get<FriendsInvitation[]>(this.baseApiUrl);
  }

  sendInvite(invite: FriendsInvitation) {
    return this.http.post(this.baseApiUrl, invite);
  }

  getPendingInvites() {
    return this.http.get<FriendsInvitation[]>(this.baseApiUrl + '/invites');
  }

  acceptInvite(friendId: string) {
    return this.http.put(this.baseApiUrl + '/acceptInvite' + `?friendId=${friendId}`, {});
  }

  declineInvite(friendId: string) {
    return this.http.put(this.baseApiUrl + '/declineInvite' + `?friendId=${friendId}`, {});
  }

  removeFriend(friendId: string) {
    return this.http.delete(this.baseApiUrl + '/deleteFriend' + `?friendId=${friendId}`);
  }
}
