import { Component, inject, model } from '@angular/core';
import { Group } from '../../../core/services/group/models/group';
import { GroupService } from '../../../core/services/group/group-service';
import { MessageService } from '../../../core/services/message/message-service';
import { User } from '../../../core/models/user';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';

@Component({
  selector: 'app-group-options',
  imports: [ImageParsePipe],
  templateUrl: './group-options.html',
  styleUrl: './group-options.css',
})
export class GroupOptions {
  selectedGroup = model<Group | null>(null);
  groupMembers = model<User[] | null>(null);
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
