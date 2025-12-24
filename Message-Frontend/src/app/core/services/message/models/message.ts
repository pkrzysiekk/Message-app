import { MessageStatus } from './message-status';
import { MessageType } from './message-type';

export interface Message {
  messageId?: number;
  senderId?: number;
  senderName?: string;
  chatId: number;
  content: string;
  sentAt?: string;
  status?: MessageStatus;
  type?: string;
}
