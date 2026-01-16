import {
  Component,
  computed,
  DestroyRef,
  effect,
  ElementRef,
  inject,
  model,
  signal,
  ViewChild,
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { form, Field, required } from '@angular/forms/signals';
import { RouterLink } from '@angular/router';

import { Chat } from '../../core/services/chat/models/chat';
import { Message } from '../../core/services/message/models/message';
import { MessageService } from '../../core/services/message/message-service';
import { UserService } from '../../core/services/user/user-service';
import { Image } from '../../core/models/image';

import { ImageParsePipe } from '../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { MessageToDataUrlPipe } from '../../shared/pipes/message-to-dataUrl-pipe';
import { takeUntil } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GroupService } from '../../core/services/group/group-service';
import { GroupOptions } from '../group-view/group-options/group-options';
import { ChatDetails } from './chat-details/chat-details';
import { ChatService } from '../../core/services/chat/chat-service';

@Component({
  selector: 'app-chat',
  imports: [
    DatePipe,
    Field,
    ImageParsePipe,
    RouterLink,
    MessageToDataUrlPipe,
    GroupOptions,
    ChatDetails,
  ],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class ChatComponent {
  @ViewChild('chatContainer') chatContainer!: ElementRef<HTMLDivElement>;
  selectedChat = model<Chat | null>(null);
  messages = signal<Message[]>([]);
  messagesFromHub = signal<Message[]>([]);
  paginationPage = signal(1);
  paginationPageSize = signal(20);
  userAvatars = signal<Map<number, Image>>(new Map());
  messageService = inject(MessageService);
  chatService = inject(ChatService);
  userService = inject(UserService);
  groupService = inject(GroupService);
  showChatDetails = signal<boolean>(false);
  destroyRef = inject(DestroyRef);

  chatMessages = computed(() => {
    const allMessages = [...this.messages(), ...this.messagesFromHub()];
    console.log(allMessages);

    const uniqueMessagesMap = new Map<number, Message>();
    allMessages.forEach((m) => {
      if (!uniqueMessagesMap.has(m.messageId!)) {
        uniqueMessagesMap.set(m.messageId!, m);
      }
    });
    return Array.from(uniqueMessagesMap.values());
  });

  avatarFor = (userId: number) => computed(() => this.userAvatars().get(userId));

  constructor() {
    this.handleChatChange();
    this.handleSignalR();
    this.loadUsersAvatar();
    this.handleMessageDelete();
    this.handleUserChatInfoUpdate();
  }

  handleChatChange() {
    effect(() => {
      const chat = this.selectedChat();
      if (!chat) return;
      this.resetChatState();

      this.messageService
        .getChatMessages(chat.id!, this.paginationPageSize())
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe((msgs) => {
          this.messages.set(msgs);
          requestAnimationFrame(() => this.scrollToBottom());
        });
    });
  }

  handleUserChatInfoUpdate() {
    effect(() => {
      const chat = this.selectedChat();
      if (!chat) return;
      if (this.chatMessages().length == 0) return;
      const lastMessage = this.chatMessages()[this.chatMessages().length - 1];
      this.chatService.updateUserChatInfo(lastMessage).subscribe();
    });
  }

  handleMessageDelete() {
    this.messageService.incomingDeletedMessage$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((msgDeleted) => {
        if (msgDeleted.chatId !== this.selectedChat()?.id) return;

        this.messages.update((list) => this.replaceMessage(list, msgDeleted));

        this.messagesFromHub.update((list) => this.replaceMessage(list, msgDeleted));
      });
  }

  onMessageDelete(msgId: number) {
    this.messageService.removeMessage(msgId);
  }

  handleSignalR() {
    this.messageService.messagesFromHub$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((msg) => {
        if (msg.chatId !== this.selectedChat()?.id) return;

        if (this.messagesFromHub().some((m) => m.messageId == msg.messageId)) return;
        this.messagesFromHub.update((list) => [...list, msg]);

        requestAnimationFrame(() => {
          if (this.isNearBottom()) {
            this.scrollToBottom();
          }
        });
      });
  }

  onScroll() {
    const el = this.chatContainer.nativeElement;
    if (el.scrollTop !== 0) return;

    const oldHeight = el.scrollHeight;
    const oldScrollTop = el.scrollTop;

    const lastMessage = this.chatMessages()[0];

    this.messageService
      .getChatMessages(this.selectedChat()!.id!, this.paginationPageSize(), lastMessage?.sentAt)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((msgs) => {
        if (!msgs.length) return;

        this.messages.update((list) => [...msgs, ...list]);
        setTimeout(() => {
          console.log(this.messages());
        }, 0);
        requestAnimationFrame(() => {
          const newHeight = el.scrollHeight;
          el.scrollTop = oldScrollTop + (newHeight - oldHeight);
        });
      });
  }

  loadUsersAvatar() {
    effect(() => {
      this.chatMessages().forEach((m) => {
        if (!m.senderId) return;
        if (this.userAvatars().has(m.senderId)) return;

        this.userService
          .getUser(m.senderId)
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe((user) => {
            if (!user?.avatar) return;

            this.userAvatars.update((map) => {
              const copy = new Map(map);
              copy.set(m.senderId!, user.avatar!);
              return copy;
            });
          });
      });
    });
  }

  messageToSendModel = model({ message: '' });
  messageToSendForm = form(this.messageToSendModel, (schema) => required(schema.message));

  onSend() {
    if (this.messageToSendForm().invalid()) return;

    this.messageService.sendTextMessage(
      this.messageToSendModel().message,
      this.selectedChat()!.id!,
    );

    this.messageToSendModel.set({ message: '' });
  }

  onSendByKey(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.onSend();
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    this.messageService.sendFile(input.files[0], this.selectedChat()!.id!);
  }

  isImage(message: Message) {
    return message.type?.startsWith('image');
  }

  resetChatState() {
    this.messages.set([]);
    this.messagesFromHub.set([]);
    this.userAvatars.set(new Map());
    this.paginationPage.set(1);
  }

  scrollToBottom() {
    const el = this.chatContainer?.nativeElement;
    if (!el) return;
    el.scrollTop = el.scrollHeight;
  }

  isNearBottom(threshold = 80) {
    const el = this.chatContainer.nativeElement;
    return el.scrollHeight - el.scrollTop - el.clientHeight < threshold;
  }

  isToday(date: string) {
    const d = new Date(date);
    const now = new Date();
    return (
      d.getDate() === now.getDate() &&
      d.getMonth() === now.getMonth() &&
      d.getFullYear() === now.getFullYear()
    );
  }

  isUserSender(message: Message) {
    return message.senderId == this.userService.localUser()?.id;
  }

  canModifyMessage(message: Message) {
    const notDeleted = message.status !== 3;
    const userRole = this.groupService.selectedUserGroupRole();
    const userId = this.userService.localUser()?.id;
    const userHasAdminRole = userRole === 1 || userRole === 2;
    const userIsSender = message.messageId == userId;
    return (userHasAdminRole || userIsSender) && notDeleted;
  }

  replaceMessage(list: Message[], updated: Message): Message[] {
    return list.map((m) => (m.messageId === updated.messageId ? updated : m));
  }

  onChatDetails = () => {
    this.showChatDetails.set(!this.showChatDetails());
  };

  ngOnDestroy() {
    this.resetChatState();
  }
}
