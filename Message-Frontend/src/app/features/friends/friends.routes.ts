import { Routes } from '@angular/router';
import { FriendsLayout } from './friends-layout/friends-layout';
import { Friends } from './friends/friends';

export const friendsRoutes: Routes = [
  {
    path: '',
    component: FriendsLayout,
    children: [
      { path: '', redirectTo: 'friends', pathMatch: 'full' },
      { path: 'friends', component: Friends },
    ],
  },
];
