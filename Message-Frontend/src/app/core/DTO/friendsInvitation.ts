import { FriendsInvitationStatus } from './FriendsInvitationStatus';

export interface FriendsInvitation {
  userId: string;
  friendId: string;
  status: FriendsInvitationStatus;
  friendsSince: string;
}
