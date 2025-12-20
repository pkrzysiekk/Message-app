import { Component, effect, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-chat',
  imports: [],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class Chat {
  private groupId = signal<number>(-1);
  route = inject(ActivatedRoute);
  constructor() {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (!id) return;
      this.groupId.set(parseInt(id));
    });

    effect(() => {});
  }
}
