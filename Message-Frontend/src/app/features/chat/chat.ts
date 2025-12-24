import { Component, computed, effect, inject, model } from '@angular/core';
import { Chat } from '../../core/services/chat/models/chat';
import { ChatService } from '../../core/services/chat/chat-service';
import { MessageService } from '../../core/services/message/message';
import { map } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { form, Field } from '@angular/forms/signals';
import { MessageType } from '../../core/services/message/models/message-type';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-chat',
  imports: [AsyncPipe, Field],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class ChatComponent {
  selectedChat = model<Chat | null>(null);
  messageService = inject(MessageService);
  messages = toSignal(this.messageService.messages$, { initialValue: [] });

  chatMessages = computed(() => {
    const chat = this.selectedChat();
    if (!chat) return [];

    return this.messages().filter((m) => m.chatId === chat.id);
  });
  messageToSendModel = model({ message: '' });
  messageToSendForm = form(this.messageToSendModel);

  onSend() {
    this.messageService.sendTextMessage(
      this.messageToSendModel().message,
      this.selectedChat()?.id!,
    );
  }

  NgOnInit() {
    this.messageService.messages.subscribe((message) => {});
  }
}
