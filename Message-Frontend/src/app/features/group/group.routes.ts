import { Routes } from '@angular/router';
import { GroupLayout } from './group-layout/group-layout';
import { Groups } from './group/group';
import { Chat } from './chat/chat';

export const groupRoutes: Routes = [
  {
    path: '',
    component: GroupLayout,
    children: [
      { path: '', redirectTo: 'groups', pathMatch: 'full' },
      { path: 'groups', component: Groups },
      { path: 'groups:id', component: Chat },
    ],
  },
];
