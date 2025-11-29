import { Routes } from '@angular/router';
import { App } from './app';
import path from 'path';
import { authRoutes } from './features/auth/auth.routes';
export const routes: Routes = [
  { path: '', component: App },
  { path: 'auth', children: authRoutes },
];
