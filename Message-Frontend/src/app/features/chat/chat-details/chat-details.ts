import { Component, model, signal } from '@angular/core';
import { Chat } from '../../../core/services/chat/models/chat';

@Component({
  selector: 'app-chat-details',
  imports: [],
  templateUrl: './chat-details.html',
  styleUrl: './chat-details.css',
})
export class ChatDetails {
  selectedChat = model<Chat | null>(null);
  closeModal = model<() => void>();
  showDeleteModal = signal<boolean>(false);

  onModalClose() {
    if (!this.closeModal()) return;
    this.closeModal()!();
  }

  onDeleteClick() {
    this.showDeleteModal.set(!this.showDeleteModal());
  }
}
