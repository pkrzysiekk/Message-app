import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AudioService {
  private messageAlert: HTMLAudioElement;

  constructor() {
    this.messageAlert = new Audio('ping.mp3');
  }

  playMessageAlert() {
    this.messageAlert.play();
  }
}
