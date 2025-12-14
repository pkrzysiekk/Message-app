import { Routes } from '@angular/router';
import { Profile } from './profile/profile';
import { ProfileLayout } from './profile-layout/profile-layout';

export const profileRoutes: Routes = [
  {
    path: '',
    component: ProfileLayout,
    children: [
      { path: '', redirectTo: 'profile', pathMatch: 'full' },
      { path: 'profile', component: Profile },
    ],
  },
];
