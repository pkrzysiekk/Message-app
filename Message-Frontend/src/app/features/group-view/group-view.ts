import { Component, computed, DestroyRef, effect, inject, model, signal } from '@angular/core';
import { Group } from '../../core/services/group/models/group';
import { Chat } from '../../core/services/chat/models/chat';
import { UserService } from '../../core/services/user/user-service';
import { ChatService } from '../../core/services/chat/chat-service';
import { GroupRole } from '../../core/services/chat/models/groupRole';
import { form, required, Field } from '@angular/forms/signals';
import { ChatComponent } from '../chat/chat';
import { MessageService } from '../../core/services/message/message-service';
import { FriendsService } from '../../core/services/friends/friends-service';
import { Friends } from '../friends/friends/friends';
import { FriendsInvitation } from '../../core/DTO/friendsInvitation';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { User } from '../../core/models/user';
import { take } from 'rxjs';
import { ImageParsePipe } from '../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { GroupService } from '../../core/services/group/group-service';
import { GroupOptions } from './group-options/group-options';

@Component({
  selector: 'app-group',
  imports: [Field, ChatComponent, ImageParsePipe, GroupOptions],
  templateUrl: './group.html',
  styleUrl: './group.css',
})
export class GroupView {
  chatService = inject(ChatService);
  friendsService = inject(FriendsService);
  messageService = inject(MessageService);
  groupService = inject(GroupService);
  userService = inject(UserService);
  destroyRef = inject(DestroyRef);
  selectedGroup = model<Group | null>(null);
  groupChats = model<Chat[] | null>(null);
  userGroupRole = model<GroupRole | null>(null);
  selectedChat = model<Chat | null>(null);
  userId = this.userService.localUser()?.id;

  showCreateChatForm = signal<boolean>(false);
  groupMembers = signal<User[]>([]);
  showInvitePeopleForm = signal<boolean>(false);
  fetchedFriends = signal<User[]>([]);
  showGroupOptions = signal<boolean>(false);

  searchTerm = signal<string>('');
  invitedIds = signal<Set<number>>(new Set());

  filteredFriends = computed<User[]>(() => {
    const term = this.searchTerm();
    return this.fetchedFriends().filter(
      (f) => f.username.startsWith(term) && !this.groupMembers().find((u) => u.id == f.id),
    );
  });

  onSearch(event: any) {
    const value = event.target.value as string;
    console.log(value);
    this.searchTerm.set(value);
  }

  GroupRole = GroupRole;
  chatTypeOptions = Object.values(GroupRole)
    .filter((v) => typeof v === 'number')
    .map((v) => ({
      value: v as GroupRole,
      label: GroupRole[v as number],
    }));

  createChatModel = model({
    chatName: '',
    ForRole: '',
  });

  fieldRequiredErrorMessage = 'This field is required';
  createChatForm = form(this.createChatModel, (schema) => {
    required(schema.chatName, { message: this.fieldRequiredErrorMessage });
    required(schema.ForRole, { message: this.fieldRequiredErrorMessage });
  });

  constructor() {
    this.fetchChats();
    this.fetchGroupMembers();
    this.listenForChatUpdates();
    this.refreshGroupMembersAfterInvite();
  }

  listenForChatUpdates() {
    this.messageService.refreshChat$.subscribe({
      next: () => {
        this.refreshChats();
      },
      error: () => {
        this.selectedChat.set(null);
        this.selectedGroup.set(null);
      },
    });
  }

  fetchGroupMembers() {
    effect(() => {
      const group = this.selectedGroup();
      const chats = this.groupChats();
      if (!group || !chats) return;
      this.invitedIds.set(new Set());
      this.groupService
        .getUsersInGroup(this.selectedGroup()?.groupId!)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: (users) => {
            this.groupMembers.set(users);
          },
        });
    });
  }

  fetchChats() {
    effect((onCleanup) => {
      const group = this.selectedGroup();
      if (!group) return;
      const sub = this.chatService.getAllUserChatsInGroup(group.groupId!).subscribe({
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

  refreshChats() {
    this.chatService.getAllUserChatsInGroup(this.selectedGroup()?.groupId!).subscribe({
      next: (fetch) => {
        console.log('fetched', fetch);
        this.groupChats.set(fetch);
      },
      error: () => {
        this.groupChats.set(null);
        this.selectedGroup.set(null);
        console.log('selectedgroup', this.selectedGroup());
      },
    });
  }

  onShowCreateChatForm() {
    this.showCreateChatForm.set(!this.showCreateChatForm());
  }

  refreshGroupMembersAfterInvite() {
    effect(() => {
      const invitedIds = this.invitedIds();
      this.groupService.getUsersInGroup(this.selectedGroup()?.groupId!).subscribe((users) => {
        this.groupMembers.set(users);
      });
    });
  }

  onChatCreate() {
    if (this.createChatForm().invalid()) return;
    console.log('Role', this.createChatModel().ForRole);
    this.chatService
      .create({
        chatName: this.createChatModel().chatName,
        forRole: parseInt(this.createChatModel().ForRole),
        groupId: this.selectedGroup()?.groupId!,
      })
      .subscribe({
        next: (chat: Chat) => {
          this.showCreateChatForm.set(false);
          this.refreshChats();
          this.messageService.sendJoinChatEvent(this.selectedGroup()?.groupId!, chat.id!);
        },
      });
  }

  onChatSelect(chat: Chat) {
    this.selectedChat.set(chat);
  }

  getFriendsToAdd() {
    this.friendsService
      .getUsersFromFriends()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (friends) => {
          this.fetchedFriends.set(friends);
          console.log(friends);
        },
      });
  }

  onInviteButtonClick() {
    this.showInvitePeopleForm.set(!this.showInvitePeopleForm());
    this.getFriendsToAdd();
  }

  onInvite(userId: number) {
    this.groupService
      .addUser(userId, {
        groupId: this.selectedGroup()?.groupId!,
        groupRole: 0,
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.invitedIds.update((set) => {
            const next = new Set(set);
            next.add(userId);
            return next;
          });
          this.messageService.sendJoinGroupEvent(this.selectedGroup()?.groupId!);
        },
      });
  }

  showGroupSettings = () => {
    this.showGroupOptions.set(!this.showGroupOptions());
  };
}
