import { groupRole } from '../models/groupRole';

export interface UserRoleRequest {
  groupId: number;
  groupRole: groupRole;
}
