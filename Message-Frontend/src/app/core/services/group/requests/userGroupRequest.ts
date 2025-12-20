import { Group } from '../models/group';

export interface UserGroupRequest {
  group: Group;
  userId: number;
}
