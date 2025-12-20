import { inject, Injectable } from '@angular/core';
import { Group } from './models/group';
import { HttpClient } from '@angular/common/http';
import { UserGroupRequest } from './requests/userGroupRequest';
import { UserRoleRequest } from './requests/userRoleRequest';
@Injectable({
  providedIn: 'root',
})
export class GroupService {
  http = inject(HttpClient);
  baseApiUrl = 'https://localhost/api/group';

  getGroup(groupId: number) {
    this.http.get<Group>(`${this.baseApiUrl}/${groupId}`);
  }

  createGroup(req: UserGroupRequest) {
    this.http.post(`${this.baseApiUrl}`, req);
  }

  updateGroup(group: Group) {
    this.http.put(`${this.baseApiUrl}`, group);
  }

  deleteGroup(groupId: number) {
    this.http.delete(`${this.baseApiUrl}/${groupId}`);
  }

  getUserGroups(page: number, pageSize: number) {
    this.http.get<Group[]>(`${this.baseApiUrl}/user-groups?page=${page}&pageSize=${pageSize}`);
  }

  addUser(userId: number, req: UserRoleRequest) {
    this.http.put(`${this.baseApiUrl}/${userId}/add`, req);
  }

  removeUser(userId: number, groupId: number) {
    this.http.delete(`${this.baseApiUrl}/${userId}/remove?groupId=${groupId}`);
  }

  updateUserRole(userId: number, req: UserRoleRequest) {
    this.http.put(`${this.baseApiUrl}/${userId}/update-group-role`, req);
  }
}
