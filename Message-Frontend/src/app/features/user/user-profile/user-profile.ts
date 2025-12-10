import { Component, effect, inject, signal } from '@angular/core';
import { UserService } from '../../../core/services/user-service';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { User } from '../../../core/models/user';
import { error } from 'console';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-user-profile',
  imports: [],
  templateUrl: './user-profile.html',
  styleUrl: './user-profile.css',
})
export class UserProfile {
  userService = inject(UserService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  fetchedUser = signal<User | null>(null);
  readonly userId = signal<string | null>(null);
  private isLoading = signal<boolean>(false);

  loadData = () => {
    this.isLoading.set(true);
    const id = this.userId();
    if (!id) {
      this.router.navigate(['/app']);
      return;
    }

    this.userService
      .getUser(id)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (fetched) => {
          this.fetchedUser.set(fetched);
          console.log(fetched);
        },
        error: (e) => console.log(e),
      });
  };

  constructor() {
    this.userId.set(this.route.snapshot.paramMap.get('id'));
    console.log(this.userId());
    this.loadData();
  }
}
