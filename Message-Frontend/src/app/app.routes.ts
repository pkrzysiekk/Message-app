import { Routes } from '@angular/router';
import { App } from './app';
import path from 'path';
import { authRoutes } from './features/auth/auth.routes';
import { unauthenticatedGuard } from './core/route-guards/unauthenticated-guard';
import { homeRoutes } from './features/home/home.routes';
import { userRoutes } from './features/user/user.routes';
import { profileRoutes } from './features/account/profile.routes';
import { friendsRoutes } from './features/friends/friends.routes';
import { groupRoutes } from './features/groups/group.routes';
export const routes: Routes = [
  { path: '', component: App },
  { path: 'auth', children: authRoutes },
  { path: 'app', children: homeRoutes },
  { path: 'user', children: userRoutes },
  { path: 'profile', children: profileRoutes },
  { path: 'friends', children: friendsRoutes },
  { path: 'groups', children: groupRoutes },
];
