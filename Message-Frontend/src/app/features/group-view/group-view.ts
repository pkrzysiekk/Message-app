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

@Component({
  selector: 'app-group',
  imports: [Field, ChatComponent, ImageParsePipe],
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

  searchTerm = signal<string>('');
  invitedIds = signal<Set<number>>(new Set());

  filteredFriends = computed<User[]>(() => {
    const term = this.searchTerm();
    return this.fetchedFriends().filter(
      (f) => f.username.startsWith(term) && !this.groupMembers().find((u) => u.id == f.id),
    );
  });

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
  }

  fetchGroupMembers() {
    effect(() => {
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
    this.chatService.getAllGroupChats(this.selectedGroup()?.groupId!).subscribe({
      next: (fetch) => {
        this.groupChats.set(fetch);
      },
    });
  }

  onShowCreateChatForm() {
    this.showCreateChatForm.set(!this.showCreateChatForm());
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

  loadFriendsAvatars() {
    effect(() => {
      this.fetchedFriends().forEach((f) => {
        this.userService
          .getUser(f.id)
          .pipe(take(1))
          .subscribe((user) => (f.avatar = user.avatar));
      });
    });
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
        },
      });
  }
}
