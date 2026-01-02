import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GroupService } from '../../../core/services/group/group-service';
import { Group } from '../../../core/services/group/models/group';
import { GroupView } from '../../group-view/group-view';
import { ClickedOutside } from '../../../shared/directives/clicked-outside/clicked-outside';
import { form, required, Field } from '@angular/forms/signals';
import { MessageService } from '../../../core/services/message/message-service';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.html',
  styleUrl: './groups.css',
  imports: [GroupView, Field],
})
export class Groups {
  groupService = inject(GroupService);
  groups = signal<Group[]>([]);
  selectedGroup = signal<Group | null>(null);
  showCreateForm = signal<boolean>(false);
  showGroupList = signal<boolean>(true);
  messageService = inject(MessageService);

  requiredFieldMessageError = `This field is required`;
  createGroupModel = signal({ groupName: '' });
  createGroupForm = form(this.createGroupModel, (schema) => {
    required(schema.groupName, { message: this.requiredFieldMessageError });
  });

  constructor() {
    this.listenForGroupUpdates();
  }

  ngOnInit() {
    this.fetchGroups();
    this.messageService.startConnection();
  }

  ngOnDestroy() {
    this.messageService.endConnection();
  }

  listenForGroupUpdates() {
    this.messageService.refreshGroups$.subscribe({
      next: () => {
        this.fetchGroups();
      },
      error: () => {
        this.selectedGroup.set(null);
      },
    });
  }

  fetchGroups() {
    this.groupService.getUserGroups().subscribe({
      next: (fetched) => {
        this.groups.set(fetched);
        console.log(this.groups());
      },
    });
  }
  onSelectedGroup(group: Group) {
    this.selectedGroup.set(group);
    this.groupService.setUserGroupRole(group.groupId!);
  }

  onShowCreateGroupForm() {
    this.showCreateForm.set(!this.showCreateForm());
  }

  onGroupCreate() {
    if (this.createGroupForm().invalid()) return;
    this.groupService.createGroup(this.createGroupModel().groupName).subscribe({
      next: (group: Group) => {
        this.showCreateForm.set(false);
        this.fetchGroups();
        this.messageService.sendJoinGroupEvent(group.groupId!);
      },
    });
  }

  onGroupListClick() {
    this.showGroupList.set(!this.showGroupList());
  }
}
