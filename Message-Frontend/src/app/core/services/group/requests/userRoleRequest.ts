import { GroupRole } from '../../chat/models/groupRole';
import { groupRole } from '../models/groupRole';

export interface UserRoleRequest {
  groupId: number;
  groupRole: GroupRole;
}
