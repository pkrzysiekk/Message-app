import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (!authService.isUserAuthenticated()) {
    router.navigate(['/auth/login']);
    return throwError(() => new HttpErrorResponse({ status: 401 }));
  }
  const newReq = req.clone({
    headers: req.headers.append('Authorization', `Bearer ${authService.authUser().authToken}`),
  });
  return next(newReq);
};
