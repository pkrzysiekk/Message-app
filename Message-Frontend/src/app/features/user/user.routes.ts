import { Routes } from '@angular/router';
import { UserProfile } from './user-profile/user-profile';
import { UserLayout } from './user-layout/user-layout';
import { Search } from './search/search';

export const userRoutes: Routes = [
  {
    path: '',
    component: UserLayout,
    children: [
      { path: 'search', component: Search },
      { path: ':id', component: UserProfile },
    ],
  },
];
