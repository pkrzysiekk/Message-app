import { Component, computed, effect, inject, model, signal } from '@angular/core';
import { Chat } from '../../core/services/chat/models/chat';
import { ChatService } from '../../core/services/chat/chat-service';
import { MessageService } from '../../core/services/message/message';
import { map } from 'rxjs';
import { AsyncPipe, DatePipe } from '@angular/common';
import { form, Field, required } from '@angular/forms/signals';
import { MessageType } from '../../core/services/message/models/message-type';
import { toSignal } from '@angular/core/rxjs-interop';
import { UserService } from '../../core/services/user/user-service';
import { Image } from '../../core/models/image';
import { UserAvatar } from './model/userAvatar';
import { ImageParsePipe } from '../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { RouterLink } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { MessageToDataUrlPipe } from '../../shared/pipes/message-to-dataUrl-pipe';
import { Message } from '../../core/services/message/models/message';

@Component({
  selector: 'app-chat',
  imports: [DatePipe, Field, ImageParsePipe, RouterLink, MessageToDataUrlPipe],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class ChatComponent {
  selectedChat = model<Chat | null>(null);
  messageService = inject(MessageService);
  userService = inject(UserService);
  messages = toSignal(this.messageService.messages$, { initialValue: [] });
  userAvatars = signal<Map<number, Image>>(new Map());
  avatarFor = (userId: number) => {
    return computed(() => this.userAvatars().get(userId));
  };
  constructor() {
    this.resetUserAvatars();
  }
  chatMessages = computed(() => {
    const chat = this.selectedChat();
    if (!chat) return [];
    this.messages().map((m) => this.loadUserAvatar(m.senderId!));
    return this.messages().filter((m) => m.chatId === chat.id);
  });

  ngOnDestroy() {
    this.messageService.endConnection();
  }
  resetUserAvatars() {
    effect(() => {
      const chat = this.selectedChat();
      this.userAvatars.set(new Map());
    });
  }

  messageToSendModel = model({ message: '' });
  messageToSendForm = form(this.messageToSendModel, (schema) => {
    required(schema.message);
  });

  onSend() {
    if (this.messageToSendForm().invalid()) return;
    this.messageService.sendTextMessage(
      this.messageToSendModel().message,
      this.selectedChat()?.id!,
    );
    this.messageToSendModel.set({ message: '' });
  }

  onSendByKey(event: KeyboardEvent) {
    console.log('eyeye');
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.onSend();
    }
  }

  loadUserAvatar(userId: number) {
    if (this.userAvatars().has(userId)) return;

    this.userService.getUser(userId).subscribe({
      next: (user) => {
        const avatar = user?.avatar;
        if (!avatar) return;
        this.userAvatars.update((map) => {
          const newMap = new Map(map);
          newMap.set(userId, user.avatar!);
          return newMap;
        });
      },
    });
  }

  isToday(date: string) {
    const d = new Date(date);
    const today = new Date();
    return (
      d.getDate() === today.getDate() &&
      d.getMonth() === today.getMonth() &&
      d.getFullYear() === today.getFullYear()
    );
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;
    const file = input.files[0];
    this.messageService.sendFile(file, this.selectedChat()?.id!);
  }
  isImage(message: Message) {
    return message.type?.startsWith('image');
  }
}
