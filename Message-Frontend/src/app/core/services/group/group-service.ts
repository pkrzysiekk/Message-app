import { inject, Injectable, signal } from '@angular/core';
import { Group } from './models/group';
import { HttpClient } from '@angular/common/http';
import { UserRoleRequest } from './requests/userRoleRequest';
import { UserService } from '../user/user-service';
import { Subject } from 'rxjs';
import { GroupRole } from '../chat/models/groupRole';
import { User } from '../../models/user';
@Injectable({
  providedIn: 'root',
})
export class GroupService {
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost/api/group';
  userService = inject(UserService);
  selectedUserGroupRole = signal<GroupRole | null>(null);

  setUserGroupRole(groupId: number) {
    this.getUserRoleInGroup(groupId).subscribe({
      next: (role) => {
        console.log(role);
        this.selectedUserGroupRole.set(role);
      },
    });
  }

  getGroup(groupId: number) {
    return this.http.get<Group>(`${this.baseApiUrl}/${groupId}`);
  }

  createGroup(groupName: string) {
    const group: Group = {
      groupName: groupName,
      groupType: 1,
    };
    return this.http.post<Group>(`${this.baseApiUrl}`, group);
  }

  updateGroup(group: Group) {
    return this.http.put(`${this.baseApiUrl}`, group);
  }

  deleteGroup(groupId: number) {
    return this.http.delete(`${this.baseApiUrl}/${groupId}`);
  }

  getUserGroups() {
    return this.http.get<Group[]>(`${this.baseApiUrl}/user-groups`);
  }

  addUser(userId: number, req: UserRoleRequest) {
    return this.http.put(`${this.baseApiUrl}/${userId}/add`, req);
  }

  removeUser(userId: number, groupId: number) {
    return this.http.delete(`${this.baseApiUrl}/${userId}/remove?groupId=${groupId}`);
  }

  updateUserRole(userId: number, req: UserRoleRequest) {
    return this.http.put(`${this.baseApiUrl}/${userId}/update-group-role`, req);
  }

  getUserRoleInGroup(groupId: number) {
    return this.http.get<GroupRole>(`${this.baseApiUrl}/${groupId}/user-role`);
  }

  getUsersInGroup(groupId: number) {
    return this.http.get<User[]>(`${this.baseApiUrl}/${groupId}/members`);
  }
}
