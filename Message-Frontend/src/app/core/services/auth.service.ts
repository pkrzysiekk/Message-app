import { computed, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { AuthUser } from '../models/auth-user.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  authUser = signal<AuthUser>({
    username: null,
    authToken: null,
  });

  isUserAuthenticated = computed<Boolean>(() => {
    return this.authUser().authToken ? true : false;
  });
}
