import { FriendsInvitationStatus } from './FriendsInvitationStatus';

export interface FriendsInvitation {
  userId: string | null;
  friendId: string;
  status: FriendsInvitationStatus;
  friendsSince: string;
}
