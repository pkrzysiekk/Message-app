import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Chat } from './models/chat';
import { Group } from '../group/models/group';
import { Subject } from 'rxjs';
import { GroupRole } from './models/groupRole';
import { UserChat } from './models/userChat';
import { UserService } from '../user/user-service';
import { Message } from '../message/models/message';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  userService = inject(UserService);
  private _selectedChat = signal<Chat | null>(null);
  selectedChat = this._selectedChat.asReadonly();
  groupChats = signal<Chat[] | null>(null);
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost/api/chat';
  //TODO: user doesn't get new msg notif via SignalR
  get(id: number) {
    return this.http.get<Chat>(`${this.baseApiUrl}/${id}`);
  }

  create(chat: Chat) {
    return this.http.post<Chat>(`${this.baseApiUrl}`, chat);
  }

  update(chat: Chat) {
    return this.http.put(`${this.baseApiUrl}`, chat);
  }

  remove(id: number) {
    return this.http.delete(`${this.baseApiUrl}/${id}`);
  }

  getAllGroupChats(groupId: number) {
    return this.http.get<Chat[]>(`${this.baseApiUrl}/group/${groupId}`);
  }

  getAllUserChats() {
    return this.http.get<Chat[]>(`${this.baseApiUrl}/user-chats`);
  }

  getAllUserChatsInGroup(groupId: number) {
    return this.http.get<Chat[]>(`${this.baseApiUrl}/${groupId}/chats`);
  }

  updateUserChatInfo(lastMessageRead: Message) {
    console.log('lastRead', lastMessageRead);
    const userId = this.userService.localUser()?.id;
    const userChat: UserChat = {
      lastMessageId: lastMessageRead.messageId!,
      lastReadAt: lastMessageRead.sentAt!,
      chatId: lastMessageRead.chatId,
      userId: userId!,
    };
    return this.http.put(`${this.baseApiUrl}/user-chat-info`, userChat);
  }

  getUserNewMessagesCountByChat(chatId: number) {
    return this.http.get<number>(`${this.baseApiUrl}/user-chat-info/${chatId}`);
  }

  setSelectedChat(chat: Chat | null) {
    this._selectedChat.set(chat);
  }
}
