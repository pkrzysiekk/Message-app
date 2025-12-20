import { Component, model, signal } from '@angular/core';
import { Group } from '../../../core/services/group/models/group';

@Component({
  selector: 'app-group',
  imports: [],
  templateUrl: './group.html',
  styleUrl: './group.css',
})
export class GroupView {
  selectedGroup = model<Group | null>(null);
}
