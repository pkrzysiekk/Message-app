import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { Message } from './models/message';
@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private connection: signalR.HubConnection;
  messages = new BehaviorSubject<Message[]>([]);
  messages$ = this.messages.asObservable();
  textEncoder = new TextEncoder();
  textDecoder = new TextDecoder();

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost/ChatHub')
      .withAutomaticReconnect()
      .build();
  }

  startConnection() {
    this.connection.on('ReceiveMessage', (message: Message) => {
      this.addMessage(message);
    });
    this.connection.start();
  }

  addMessage(message: Message) {
    if (message.type === 'text/plain') {
      const decodedContent = this.decodeBase64Utf8(message.content);
      message.content = decodedContent;
    }

    this.messages.next([...this.messages.getValue(), message]);
  }

  sendTextMessage(messageContent: string, chatId: number) {
    const encodedMessage = this.decodeUtf8Base64(messageContent);
    const message: Message = {
      chatId: chatId,
      content: encodedMessage,
      type: 'text/plain',
    };
    this.connection.invoke('SendMessage', message);
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
