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
  messages = new BehaviorSubject<Message[]>([]);
  messages$ = this.messages.asObservable();
  incomingMessage$ = new Subject<Message>();
  messagesFromHub$ = this.incomingMessage$.asObservable();
  textEncoder = new TextEncoder();
  textDecoder = new TextDecoder();
  fileReader = new FileReader();

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost/ChatHub')
      .withAutomaticReconnect()
      .build();
  }

  getChatMessages(chatId: number, page: number, pageSize: number) {
    return this.http
      .get<Message[]>(`${this.baseApiUrl}/${chatId}/messages?page=${page}&pageSize=${pageSize}`)
      .pipe(
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
    this.connection.start();
  }

  endConnection() {
    this.connection.stop().then(() => this.messages.next([]));
  }

  addMessage(message: Message) {
    console.log('message', message);
    if (message.type === 'text/plain') {
      const decodedContent = this.decodeBase64Utf8(message.content);
      message.content = decodedContent;
      console.log('decoded,', message);
    }
    //this.messages.next([...this.messages.getValue(), message]);
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

  async sendFile(file: File, chatId: number) {
    this.fileReader.readAsDataURL(file);
    this.fileReader.onload = () => {
      const content = this.fileReader.result as string;
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
