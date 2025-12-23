import { Component, model } from '@angular/core';
import { Chat } from '../../core/services/chat/models/chat';

@Component({
  selector: 'app-chat',
  imports: [],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class ChatComponent {
  selectedChat = model<Chat | null>(null);
}
