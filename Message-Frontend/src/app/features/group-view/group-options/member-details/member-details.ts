import { Component, effect, inject, model, signal } from '@angular/core';
import { User } from '../../../../core/models/user';
import { ImageParsePipe } from '../../../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { GroupService } from '../../../../core/services/group/group-service';
import { GroupRole } from '../../../../core/services/chat/models/groupRole';
import { Group } from '../../../../core/services/group/models/group';
import { MessageService } from '../../../../core/services/message/message-service';
import { UserService } from '../../../../core/services/user/user-service';

@Component({
  selector: 'app-member-details',
  imports: [ImageParsePipe],
  templateUrl: './member-details.html',
  styleUrl: './member-details.css',
})
export class MemberDetails {
  selectedUser = model<User | null>(null);
  selectedGroup = model<Group | null>(null);
  selectedRole = model<number | null>(null);
  userService = inject(UserService);
  groupService = inject(GroupService);
  messageService = inject(MessageService);
  fetchedUserRole = signal<GroupRole | null>(null);
  GroupRole = GroupRole;
  chatTypeOptions = Object.values(GroupRole)
    .filter((v) => typeof v === 'number')
    .map((v) => ({
      value: v as GroupRole,
      label: GroupRole[v as number],
    }));

  closeModal = model<() => void>();
  constructor() {
    this.fetchUserRole();
  }

  fetchUserRole() {
    effect(() => {
      this.groupService
        .getUserRoleInGroup(this.selectedGroup()?.groupId!, this.selectedUser()?.id!)
        .subscribe({
          next: (role) => {
            console.log(role);
            this.fetchedUserRole.set(role);
          },
        });
    });
  }

  onModalClose() {
    this.closeModal()!();
  }

  onRoleSelection(event: any) {
    const value = parseInt(event.target.value);
    console.log(value);
    this.selectedRole.set(value);
  }

  onRoleUpdate() {
    if (this.selectedRole() == null) return;
    console.log('Update');
    this.groupService
      .updateUserRole(this.selectedUser()?.id!, {
        groupId: this.selectedGroup()?.groupId!,
        groupRole: this.selectedRole()!,
      })
      .subscribe({
        next: () => {
          this.messageService.SendUserRoleUpdatedEvent(
            this.selectedUser()?.id!,
            this.selectedGroup()?.groupId!,
          );
          this.onModalClose();
        },
      });
  }

  onUserRemoval() {
    this.groupService
      .removeUser(this.selectedUser()?.id!, this.selectedGroup()?.groupId!)
      .subscribe({
        next: () => {
          this.messageService.sendUserRemovedEvent(
            this.selectedUser()?.id!,
            this.selectedGroup()?.groupId!,
          );
          this.onModalClose();
        },
      });
  }
}
