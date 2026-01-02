import { Component, inject, model, signal } from '@angular/core';
import { Chat } from '../../../core/services/chat/models/chat';
import { MessageService } from '../../../core/services/message/message-service';
import { GroupService } from '../../../core/services/group/group-service';

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
  messageService = inject(MessageService);
  groupService = inject(GroupService);

  onModalClose() {
    if (!this.closeModal()) return;
    this.closeModal()!();
  }

  onDeleteClick() {
    this.showDeleteModal.set(!this.showDeleteModal());
  }

  removeChat() {
    this.messageService.removeChat(
      this.groupService.selectedGroup()?.groupId!,
      this.selectedChat()?.id!,
    );
    this.onModalClose();
  }
}
