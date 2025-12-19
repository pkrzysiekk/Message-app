import { Routes } from '@angular/router';
import { GroupLayout } from './group-layout/group-layout';
import { Group } from './group/group';

export const groupRoutes: Routes = [
  {
    path: '',
    component: GroupLayout,
    children: [
      { path: '', redirectTo: 'groups', pathMatch: 'full' },
      { path: 'groups', component: Group },
    ],
  },
];
