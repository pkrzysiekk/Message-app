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
    this.messages.next([...this.messages.getValue(), message]);
  }

  sendMessage(message: Message) {
    this.connection.invoke('SendMessage', message);
  }
}
