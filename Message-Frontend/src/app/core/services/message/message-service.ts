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
          const decodedContent = this.decodeBase64Utf8(msg.content);
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
    this.connection.on('ReceiveConnectionStateChanged', () => {
      this.connection.invoke('RefreshConnectionState');
    });
    this.connection.start();
  }

  endConnection() {
    this.connection.stop();
  }

  notifyMessageDelete(message: Message) {
    const encodedContent = this.decodeBase64Utf8(message.content);
    message.content = encodedContent;
    this.incomingDeletedMessage.next(message);
  }

  sendGroupStateChanged(groupId: number) {
    this.connection.invoke('SendConnectionStateChanged', groupId);
  }

  addMessage(message: Message) {
    console.log('message', message);
    if (message.type === 'text/plain') {
      const decodedContent = this.decodeBase64Utf8(message.content);
      message.content = decodedContent;
    }
    console.log(message);
    this.incomingMessage$.next(message);
  }

  sendTextMessage(messageContent: string, chatId: number) {
    const encodedMessage = this.decodeUtf8Base64(messageContent);
    const message: Message = {
      chatId: chatId,
      content: encodedMessage,
      type: 'text/plain',
    };
    return this.connection.invoke('SendMessage', message);
  }

  removeMessage(messageId: number) {
    this.connection.invoke('RemoveMessage', messageId);
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

  decodeBase64Utf8(base64: string) {
    const binary = atob(base64);
    const bytes = Uint8Array.from(binary, (c) => c.charCodeAt(0));
    return this.textDecoder.decode(bytes);
  }

  decodeUtf8Base64(text: string) {
    const encodedContent = this.textEncoder.encode(text);
    const encodedMessage = btoa(String.fromCharCode(...encodedContent));
    return encodedMessage;
  }
}
