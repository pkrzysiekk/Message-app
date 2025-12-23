import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GroupService } from '../../../core/services/group/group-service';
import { Group } from '../../../core/services/group/models/group';
import { GroupView } from '../../group-view/group-view';
import { ClickedOutside } from '../../../shared/directives/clicked-outside/clicked-outside';
import { form, required, Field } from '@angular/forms/signals';
import { MessageService } from '../../../core/services/message/message';

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
  page = signal<number>(1);
  pageSize = signal<number>(10);
  showCreateForm = signal<boolean>(false);
  showGroupList = signal<boolean>(true);
  messageService = inject(MessageService);

  requiredFieldMessageError = `This field is required`;
  createGroupModel = signal({ groupName: '' });
  createGroupForm = form(this.createGroupModel, (schema) => {
    required(schema.groupName, { message: this.requiredFieldMessageError });
  });

  constructor() {}

  ngOnInit() {
    this.fetchGroups();
    this.messageService.startConnection();
  }

  fetchGroups() {
    this.groupService.getUserGroups(this.page(), this.pageSize()).subscribe({
      next: (fetched) => {
        this.groups.set(fetched);
        console.log(this.groups());
      },
    });
  }
  onSelectedGroup(group: Group) {
    this.selectedGroup.set(group);
  }

  onShowCreateGroupForm() {
    this.showCreateForm.set(!this.showCreateForm());
  }

  onGroupCreate() {
    if (this.createGroupForm().invalid()) return;
    this.groupService.createGroup(this.createGroupModel().groupName).subscribe({
      next: () => {
        this.showCreateForm.set(false);
        this.fetchGroups();
      },
    });
  }

  onGroupListClick() {
    this.showGroupList.set(!this.showGroupList());
  }
}
