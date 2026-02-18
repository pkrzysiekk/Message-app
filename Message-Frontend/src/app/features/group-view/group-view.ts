import {
  Component,
  computed,
  DestroyRef,
  effect,
  inject,
  model,
  Signal,
  signal,
} from '@angular/core';
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
import { forkJoin, map, switchMap, take } from 'rxjs';
import { ImageParsePipe } from '../../shared/pipes/image-parse-pipe/image-parse-pipe';
import { GroupService } from '../../core/services/group/group-service';
import { GroupOptions } from './group-options/group-options';
import { error } from 'console';

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
  refreshSignal = signal(0);
  selectedGroup = signal<Group | null>(null);
  groupChats = signal<Chat[] | null>(null);
  userGroupRole = model<GroupRole | null>(null);
  selectedChat: Signal<Chat | null>;
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

  CheckNewMessageCount() {
    effect(() => {
      this.groupChats()?.map((c) => {
        this.chatService
          .getUserNewMessagesCountByChat(c.id!)
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: (messageCount) => {
              c.newMessageCount = messageCount;
              console.log('updated chat', c);
            },
          });
      });
    });
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
    this.selectedGroup = this.groupService.selectedGroup;
    this.selectedChat = this.chatService.selectedChat;
    this.groupChats = this.chatService.groupChats;
    this.fetchChats();
    this.fetchGroupMembers();
    this.listenForChatUpdates();
    this.refreshGroupMembersAfterInvite();

    effect(() => {
      if (!this.selectedGroup()) this.chatService.setSelectedChat(null);
    });
  }

  listenForChatUpdates() {
    this.messageService.refreshChat$.subscribe({
      next: () => {
        this.refreshSignal.update((v) => v + 1);
      },
      error: () => {
        this.chatService.setSelectedChat(null);
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
      const refreshSignal = this.refreshSignal();
      if (!group) {
        this.groupChats.set([]);
        return;
      }

      const sub = this.chatService
        .getAllUserChatsInGroup(group.groupId!)
        .pipe(
          switchMap((chats) =>
            forkJoin(
              chats.map((c) =>
                this.chatService.getUserNewMessagesCountByChat(c.id!).pipe(
                  map((count) => ({
                    ...c,
                    newMessageCount: count,
                  })),
                ),
              ),
            ),
          ),
        )
        .subscribe((updatedChats) => {
          this.groupChats.set(updatedChats);
        });

      onCleanup(() => sub.unsubscribe());
    });
  }

  refreshChats() {
    this.chatService.getAllUserChatsInGroup(this.selectedGroup()?.groupId!).subscribe({
      next: (fetch) => {
        this.groupChats.set(fetch);
        if (!fetch.some((c) => c.id == this.selectedChat()?.id))
          this.chatService.setSelectedChat(null);
      },
      error: () => {
        this.selectedGroup.set(null);
        this.chatService.setSelectedChat(null);
      },
    });
  }

  onShowCreateChatForm() {
    this.showCreateChatForm.set(!this.showCreateChatForm());
  }

  refreshGroupMembersAfterInvite() {
    effect(() => {
      const invitedIds = this.invitedIds();
      this.groupService.getUsersInGroup(this.selectedGroup()?.groupId!).subscribe({
        next: (users) => {
          this.groupMembers.set(users);
        },
        error: () => {
          this.selectedGroup.set(null);
        },
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
    chat.newMessageCount = 0;
    this.chatService.setSelectedChat(chat);
    this.checkIfAllMessagesRead();
  }

  checkIfAllMessagesRead() {
    const allChatsRead = this.groupChats()?.every((c) => c.newMessageCount == 0);
    if (allChatsRead)
      this.groupService.selectedGroup.update((g) => {
        const updatedGroup = g;
        updatedGroup!.hasNewMessages = false;
        return updatedGroup;
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
          this.messageService.sendJoinGroupEvent(this.selectedGroup()?.groupId!);
        },
      });
  }

  showGroupSettings = () => {
    this.showGroupOptions.set(!this.showGroupOptions());
  };
}
