import { Component } from '@angular/core';
import { HomeLayout } from './home-layout/home-layout';
import { Routes } from '@angular/router';
import { Home } from './home/home';

export const homeRoutes: Routes = [
  {
    path: '',
    component: HomeLayout,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: Home },
    ],
  },
];
