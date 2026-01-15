import { inject, Injectable, Sanitizer } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, map, Subject } from 'rxjs';
import { Message } from './models/message';
import { DomSanitizer } from '@angular/platform-browser';
import { HttpClient } from '@angular/common/http';
@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private connection: signalR.HubConnection;
  private refreshGroups = new Subject<void>();
  refreshGroups$ = this.refreshGroups.asObservable();
  private refreshChat = new Subject<void>();
  refreshChat$ = this.refreshChat.asObservable();

  baseApiUrl = 'https://localhost/api/message';
  http = inject(HttpClient);
  incomingMessage$ = new Subject<Message>();
  incomingDeletedMessage = new Subject<Message>();
  incomingDeletedMessage$ = this.incomingDeletedMessage.asObservable();

  messagesFromHub$ = this.incomingMessage$.asObservable();
  textEncoder = new TextEncoder();
  textDecoder = new TextDecoder();

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost/ChatHub')
      .withAutomaticReconnect()
      .build();
  }

  getChatMessages(chatId: number, pageSize: number, messageSince: string | undefined = undefined) {
    const messageRequestUrl = messageSince
      ? `${this.baseApiUrl}/${chatId}/messages?messageSince=${messageSince}&pageSize=${pageSize}`
      : `${this.baseApiUrl}/${chatId}/messages?pageSize=${pageSize}`;
    return this.http.get<Message[]>(messageRequestUrl).pipe(
      map((msgs) =>
        msgs.map((msg) => {
          if (msg.type !== 'text/plain') return msg;
          const decodedContent = this.decodeBase64ToUtf8(msg.content);
          msg.content = decodedContent;
          return msg;
        }),
      ),
    );
  }

  startConnection() {
    this.connection.on('ReceiveMessage', (message: Message) => {
      this.addMessage(message);
    });
    this.connection.on('ReceiveMessageRemovedEvent', (message: Message) => {
      this.notifyMessageDelete(message);
    });
    this.connection.on('ReceiveAddToGroupEvent', (groupId: number) => {
      this.connection.invoke('JoinGroup', groupId).then(() => this.refreshGroups.next());
    });
    this.connection.on('ReceiveAddToChatEvent', (chatId: number) => {
      this.connection.invoke('JoinChat', chatId).then(() => this.refreshChat.next());
    });
    this.connection.on('ReceiveRemovedFromGroupEvent', (groupId: number) => {
      this.refreshGroups.next();
      this.refreshChat.next();
    });
    this.connection.on('ReceiveGroupRoleChangedEvent', (groupId: number) => {
      this.connection.invoke('RefreshRoles', groupId).then(() => {
        this.refreshChat.next();
      });
    });
    this.connection.on('ReceiveChatDeletedEvent', (groupId: number) => {
      this.refreshChat.next();
    });
    this.connection.start().catch((err) => console.log(err));
  }

  endConnection() {
    this.connection.stop();
    console.log('ended');
  }

  sendJoinChatEvent(groupId: number, chatId: number) {
    this.connection.invoke('SendNewChatRequest', groupId, chatId);
  }

  sendJoinGroupEvent(groupId: number) {
    this.connection
      .invoke('SendNewGroupRequest', groupId)
      .catch((err) => console.error('Error sending group request:', err));
  }

  notifyMessageDelete(message: Message) {
    const encodedContent = this.decodeBase64ToUtf8(message.content);
    message.content = encodedContent;
    this.incomingDeletedMessage.next(message);
  }

  sendGroupStateChanged(groupId: number) {
    this.connection.invoke('SendConnectionStateChanged', groupId);
  }

  sendGroupRemovedEvent(groupId: number) {
    this.connection.invoke('SendGroupRemovedEvent', groupId);
  }

  addMessage(message: Message) {
    console.log('message', message);
    if (message.type === 'text/plain') {
      const decodedContent = this.decodeBase64ToUtf8(message.content);
      message.content = decodedContent;
    }
    console.log(message);
    this.incomingMessage$.next(message);
  }

  sendTextMessage(messageContent: string, chatId: number) {
    const encodedMessage = this.encodeUtf8ToBase64(messageContent);
    const message: Message = {
      chatId: chatId,
      content: encodedMessage,
      type: 'text/plain',
    };
    console.log('to Send,', message);
    return this.connection.invoke('SendMessage', message);
  }

  removeMessage(messageId: number) {
    this.connection.invoke('RemoveMessage', messageId);
  }

  SendUserRoleUpdatedEvent(userId: number, groupId: number) {
    this.connection.invoke('SendUserRoleUpdatedEvent', userId, groupId);
  }

  sendUserRemovedEvent(userIdToRemove: number, groupId: number) {
    this.connection.invoke('SendUserRemovedEvent', userIdToRemove, groupId);
  }

  sendChatRemovedEvent(groupId: number, chatId: number) {
    this.connection.invoke('SendChatRemovedEvent', groupId, chatId);
  }

  async sendFile(file: File, chatId: number) {
    const fileReader = new FileReader();
    fileReader.readAsDataURL(file);
    fileReader.onload = () => {
      const content = fileReader.result as string;
      const base64 = content.replace('data:', '').replace(/^.+,/, '');
      const message: Message = {
        chatId: chatId,
        content: base64,
        type: file.type,
      };
      this.connection.invoke('SendMessage', message);
    };
  }

  decodeBase64ToUtf8(base64: string) {
    const binary = atob(base64);
    const buffer = new Uint8Array(binary.length);
    for (let i = 0; i < buffer.length; i++) {
      buffer[i] = binary.charCodeAt(i);
    }
    return this.textDecoder.decode(buffer);
  }

  encodeUtf8ToBase64(text: string) {
    const bytes = this.textEncoder.encode(text);
    let binary = '';
    for (const b of bytes) {
      binary += String.fromCharCode(b);
    }
    return btoa(binary);
  }
}
