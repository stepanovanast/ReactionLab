import { Component, HostBinding, OnInit, inject } from '@angular/core';
import { NavbarComponent } from '../../app/navbar/navbar.component';

import { AuthService } from '../../services/auth.service';
import { ApiService, Badge } from '../../services/api.service';

const DEFAULT_BADGES: Badge[] = [
  { id: 1, name: 'Early Bird',       description: 'Join ReactionLab',                              earned: false },
  { id: 2, name: 'Iron Alchemist',   description: 'Complete the Iron Sulfide Formation reaction',   earned: false },
  { id: 3, name: 'Rising Chemist',   description: 'Complete your second topic',                     earned: false },
  { id: 4, name: 'Lab Veteran',      description: 'Complete your third topic',                      earned: false },
  { id: 5, name: 'Chemistry Master', description: 'Complete your fourth topic',                     earned: false },
  { id: 6, name: 'Molecular Vision', description: 'Switch visualization mode for the first time',   earned: false },
  { id: 7, name: 'Full Circuit',     description: 'Reach the final step of any reaction',           earned: false },
  { id: 8, name: 'Completionist',    description: 'Complete all topics in the lab',                 earned: false },
];

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

  badges: Badge[] = DEFAULT_BADGES.map(b => ({ ...b }));

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
      next: (badges) => {
        // Merge API earned state into the default list
        const earnedSet = new Set(badges.filter(b => b.earned).map(b => b.id));
        this.badges = DEFAULT_BADGES.map(b => ({ ...b, earned: earnedSet.has(b.id) }));
      },
      error: () => {
        // Keep badges visible as unearned if the call fails (e.g. expired token)
        this.badges = DEFAULT_BADGES.map(b => ({ ...b }));
      }
    });
  }

  signOut(): void {
    this.authService.logout();
  }

  onSidebarCollapse(collapsed: boolean): void {}
}
