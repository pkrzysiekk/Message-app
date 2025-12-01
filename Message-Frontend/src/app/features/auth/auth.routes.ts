import { Routes } from '@angular/router';
import { Login } from './login/login';
import path from 'path';
import { AuthLayout } from './auth-layout/auth-layout';
import { Register } from './register/register';

export const authRoutes: Routes = [
  {
    path: '',
    component: AuthLayout,
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', component: Login },
      { path: 'register', component: Register },
    ],
  },
];
