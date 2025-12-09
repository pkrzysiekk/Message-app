import { Routes } from '@angular/router';
import { UserProfile } from './user-profile/user-profile';

export const userRoutes: Routes = [{ path: ':id', component: UserProfile }];
