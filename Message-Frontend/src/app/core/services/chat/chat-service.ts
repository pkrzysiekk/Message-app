import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Chat } from './models/chat';
import { Group } from '../group/models/group';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost/api/chat';

  get(id: number) {
    return this.http.get<Chat>(`${this.baseApiUrl}/${id}`);
  }

  create(chat: Chat) {
    return this.http.post(`${this.baseApiUrl}`, chat);
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
}
