import { Component, inject, model, signal } from '@angular/core';
import { Group } from '../../../core/services/group/models/group';
import { GroupService } from '../../../core/services/group/group-service';
import { MessageService } from '../../../core/services/message/message-service';
import { User } from '../../../core/models/user';
import { ImageParsePipe } from '../../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { MemberDetails } from './member-details/member-details';

@Component({
  selector: 'app-group-options',
  imports: [ImageParsePipe, MemberDetails],
  templateUrl: './group-options.html',
  styleUrl: './group-options.css',
})
export class GroupOptions {
  selectedGroup = model<Group | null>(null);
  selectedUser = signal<User | null>(null);
  groupMembers = model<User[] | null>(null);
  messageService = inject(MessageService);
  groupService = inject(GroupService);
  closeModal = model<() => void>();
  showConfirmModal = model<boolean>(false);
  showMemberDetailsModal = model<boolean>(false);

  onModalClose() {
    console.log(this.closeModal());
    if (!this.closeModal()) return;
    this.closeModal()!();
  }

  onGroupDelete() {
    this.showConfirmModal.set(!this.showConfirmModal());
  }

  onGroupDeleteConfirm() {
    this.messageService.deleteGroup(this.selectedGroup()?.groupId!);
    this.onGroupDelete();
    this.onModalClose();
  }

  toggleMemberDetailsModal = () => {
    this.showMemberDetailsModal.set(!this.showMemberDetailsModal());
  };

  onSelectedUser(user: User) {
    this.selectedUser.set(user);
    this.toggleMemberDetailsModal();
  }
}
