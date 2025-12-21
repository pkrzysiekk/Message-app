import { GroupType } from './groupType';

export interface Group {
  groupId?: number;
  groupName: string;
  createdAt?: string;
  groupType: GroupType;
  usersInGroup?: number[];
}
