import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

export const unauthenticatedGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  console.log('Auth guard!', authService.isUserAuthenticated());
  return !authService.isUserAuthenticated();
};
