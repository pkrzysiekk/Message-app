import { Avatar } from './avatar';

export interface User {
  id: number;
  username: string;
  email: string;
  lastSeen: string;
  avatar?: Avatar;
  isOnline: boolean;
}
