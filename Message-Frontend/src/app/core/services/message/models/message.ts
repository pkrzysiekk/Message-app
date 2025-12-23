import { MessageStatus } from './message-status';
import { MessageType } from './message-type';

export interface Message {
  messageId?: number;
  senderId: number;
  chatId: number;
  content: Blob;
  sentAt?: string;
  status?: MessageStatus;
  type?: MessageType;
}
