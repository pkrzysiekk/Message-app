import { Component, inject, model } from '@angular/core';
import { Chat } from '../../core/services/chat/models/chat';
import { ChatService } from '../../core/services/chat/chat-service';

@Component({
  selector: 'app-chat',
  imports: [],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class ChatComponent {
  selectedChat = model<Chat | null>(null);
  chatService = inject(ChatService);

  NgOnInit() {}
}
