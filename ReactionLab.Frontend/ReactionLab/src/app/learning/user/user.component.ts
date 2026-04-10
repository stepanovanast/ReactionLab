import { Component, HostBinding, OnInit, inject } from '@angular/core';
import { NavbarComponent } from '../../app/navbar/navbar.component';

import { AuthService } from '../../services/auth.service';
import { ApiService, Badge } from '../../services/api.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  private authService = inject(AuthService);
  private apiService = inject(ApiService);

  badges: Badge[] = [];

  private readonly avatarMap: Record<number, string> = {
    1: '🧪', 2: '⚗️', 3: '🔬', 4: '🧬', 5: '⚡',
  };

  get userName(): string {
    return this.authService.getCurrentUser()?.name ?? '';
  }

  get avatarEmoji(): string {
    const id = this.authService.getCurrentUser()?.avatarId ?? 1;
    return this.avatarMap[id] ?? '🧪';
  }

  ngOnInit(): void {
    this.apiService.getUserBadges().subscribe({
      next: (badges) => this.badges = badges,
      error: () => this.badges = []
    });
  }

  onSidebarCollapse(collapsed: boolean): void {}
}
