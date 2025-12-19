import { GroupType } from './groupType';

export interface Group {
  id: number;
  groupName: string;
  createdAt: string;
  groupType: GroupType;
  usersInGroup: number[];
}
