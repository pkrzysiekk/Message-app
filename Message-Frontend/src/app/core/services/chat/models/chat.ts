import { GroupRole } from './groupRole';

export interface Chat {
  id: number;
  groupId: number;
  chatName: string;
  forRole: GroupRole;
}
