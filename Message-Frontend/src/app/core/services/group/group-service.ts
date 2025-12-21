import { inject, Injectable } from '@angular/core';
import { Group } from './models/group';
import { HttpClient } from '@angular/common/http';
import { UserGroupRequest } from './requests/userGroupRequest';
import { UserRoleRequest } from './requests/userRoleRequest';
import { UserService } from '../user/user-service';
@Injectable({
  providedIn: 'root',
})
export class GroupService {
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost/api/group';
  userService = inject(UserService);

  getGroup(groupId: number) {
    return this.http.get<Group>(`${this.baseApiUrl}/${groupId}`);
  }

  createGroup(groupName: string) {
    const req: UserGroupRequest = {
      userId: this.userService.localUser()?.id!,
      group: {
        groupName: groupName,
        groupType: 1,
      },
    };
    return this.http.post(`${this.baseApiUrl}`, req);
  }

  updateGroup(group: Group) {
    return this.http.put(`${this.baseApiUrl}`, group);
  }

  deleteGroup(groupId: number) {
    return this.http.delete(`${this.baseApiUrl}/${groupId}`);
  }

  getUserGroups(page: number, pageSize: number) {
    return this.http.get<Group[]>(
      `${this.baseApiUrl}/user-groups?page=${page}&pageSize=${pageSize}`,
    );
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
}
