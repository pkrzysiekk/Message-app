import { Component, effect, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GroupService } from '../../../core/services/group/group-service';
import { Group } from '../../../core/services/group/models/group';
import { GroupView } from '../../group-view/group-view';
import { ClickedOutside } from '../../../shared/directives/clicked-outside/clicked-outside';
import { form, required, Field } from '@angular/forms/signals';
import { MessageService } from '../../../core/services/message/message-service';
import { sign } from 'crypto';
import { forkJoin, map, switchMap } from 'rxjs';
import { ChatService } from '../../../core/services/chat/chat-service';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.html',
  styleUrl: './groups.css',
  imports: [GroupView, Field],
})
export class Groups {
  groupService = inject(GroupService);
  groups = this.groupService.groups;
  selectedGroup = signal<Group | null>(null);
  showCreateForm = signal<boolean>(false);
  showGroupList = signal<boolean>(true);
  messageService = inject(MessageService);
  initialGroupSelected = signal<boolean>(false);
  chatService = inject(ChatService);

  requiredFieldMessageError = `This field is required`;
  createGroupModel = signal({ groupName: '' });
  createGroupForm = form(this.createGroupModel, (schema) => {
    required(schema.groupName, { message: this.requiredFieldMessageError });
  });

  constructor() {
    this.listenForGroupUpdates();
    this.selectFirstGroupFallback();
    this.selectedGroup = this.groupService.selectedGroup;
    this.listenForSelectedGroupUpdates();
  }

  selectFirstGroupFallback() {
    effect(() => {
      console.log('group changed!');
      if (!this.selectedGroup() && this.groups().length > 0) this.onSelectedGroup(this.groups()[0]);
    });
  }

  listenForSelectedGroupUpdates() {
    effect(() => {
      const selectedGroup = this.groupService.selectedGroup();
      const selectedChat = this.chatService.selectedChat();
      if (!selectedGroup) return;
      this.groups.update((list) => {
        const newList = [...list];
        const index = list.findIndex((g) => g.groupId == selectedGroup.groupId);
        newList.splice(index, 1, selectedGroup);
        return newList;
      });
    });
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
    this.groupService
      .getUserGroups()
      .pipe(
        switchMap((groups) => {
          return forkJoin(
            groups.map((g) =>
              this.groupService
                .groupHasNewMessages(g.groupId!)
                .pipe(map((hasNew) => ({ ...g, hasNewMessages: hasNew }))),
            ),
          );
        }),
      )
      .subscribe((groupsWithFlags) => {
        this.groups.set(groupsWithFlags);
      });
  }

  onSelectedGroup(group: Group) {
    this.groupService.setSelectedGroup(group);
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
