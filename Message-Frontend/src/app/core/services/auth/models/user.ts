export interface User {
  id: number;
  username: string;
  email: string;
  lastSeen: string;
  avatar: Uint8Array;
  isOnline: boolean;
}
