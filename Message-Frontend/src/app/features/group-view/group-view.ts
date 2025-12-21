import { Component, effect, inject, model, signal } from '@angular/core';
import { Group } from '../../core/services/group/models/group';
import { Chat } from '../../core/services/chat/models/chat';
import { UserService } from '../../core/services/user/user-service';
import { ChatService } from '../../core/services/chat/chat-service';

@Component({
  selector: 'app-group',
  imports: [],
  templateUrl: './group.html',
  styleUrl: './group.css',
})
export class GroupView {
  chatService = inject(ChatService);
  selectedGroup = model<Group | null>(null);
  groupChats = model<Chat[] | null>(null);

  constructor() {
    this.fetchChats();
  }

  fetchChats() {
    effect((onCleanup) => {
      const group = this.selectedGroup();
      if (!group) return;
      const sub = this.chatService.getAllGroupChats(group.groupId!).subscribe({
        next: (chats) => {
          console.log('chats', chats);
          this.groupChats.set(chats);
        },
      });
      onCleanup(() => {
        sub.unsubscribe();
      });
    });
  }
}
