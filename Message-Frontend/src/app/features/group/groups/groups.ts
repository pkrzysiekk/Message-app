import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GroupService } from '../../../core/services/group/group-service';
import { Group } from '../../../core/services/group/models/group';
import { GroupView } from '../../../feautures/group/group/group';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.html',
  styleUrl: './groups.css',
  imports: [GroupView],
})
export class Groups {
  groupService = inject(GroupService);
  groups = signal<Group[]>([]);
  selectedGroup = signal<Group | null>(null);
  page = signal<number>(1);
  pageSize = signal<number>(10);

  constructor() {
    this.fetchGroups();
  }

  fetchGroups() {
    this.groupService.getUserGroups(this.page(), this.pageSize()).subscribe({
      next: (fetched) => {
        this.groups.set([...this.groups(), ...fetched]);
        console.log(this.groups());
      },
    });
  }
  onSelectedGroup(group: Group) {
    this.selectedGroup.set(group);
  }
}
