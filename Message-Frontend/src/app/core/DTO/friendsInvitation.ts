import { FriendsInvitationStatus } from './FriendsInvitationStatus';

export interface FriendsInvitation {
  userId: number;
  friendId: number;
  status: FriendsInvitationStatus;
  friendsSince: string;
}
