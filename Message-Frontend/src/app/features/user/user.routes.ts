import { Routes } from '@angular/router';
import { UserProfile } from './user-profile/user-profile';
import { UserLayout } from './user-layout/user-layout';

export const userRoutes: Routes = [
  { path: '', component: UserLayout, children: [{ path: ':id', component: UserProfile }] },
];
