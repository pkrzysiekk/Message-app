import { Component, inject, model } from '@angular/core';
import { Group } from '../../../core/services/group/models/group';
import { GroupService } from '../../../core/services/group/group-service';
import { MessageService } from '../../../core/services/message/message-service';

@Component({
  selector: 'app-group-options',
  imports: [],
  templateUrl: './group-options.html',
  styleUrl: './group-options.css',
})
export class GroupOptions {
  selectedGroup = model<Group | null>(null);
  messageService = inject(MessageService);
  groupService = inject(GroupService);
  closeModal = model<() => void>();

  onModalClose() {
    console.log(this.closeModal());
    if (!this.closeModal()) return;
    this.closeModal()!();
  }

  onGroupDelete() {
    this.messageService.deleteGroup(this.selectedGroup()?.groupId!);
    this.onModalClose();
  }
}
