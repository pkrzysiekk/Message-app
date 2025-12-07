import { Image } from './image';

export interface User {
  id: number;
  username: string;
  email: string;
  lastSeen: string;
  avatar?: Image;
  isOnline: boolean;
}
