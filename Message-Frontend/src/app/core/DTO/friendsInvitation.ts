import { FriendsInvitationStatus } from './FriendsInvitationStatus';

export interface FriendsInvitation {
  friendId: string;
  status: FriendsInvitationStatus;
  friendsSince: string;
}
