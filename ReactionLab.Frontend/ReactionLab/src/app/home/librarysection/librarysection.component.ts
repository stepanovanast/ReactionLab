import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-librarysection',
  standalone: true,
  imports: [],
  templateUrl: './librarysection.component.html',
  styleUrl: './librarysection.component.css'
})
export class LibrarysectionComponent implements OnInit {
  private router = inject(Router);
  private apiService = inject(ApiService);
  private authService = inject(AuthService);

  private topicMap = new Map<string, { topicId: number; reactionId?: number }>();

  ngOnInit(): void {
    this.apiService.getTopicMap().subscribe({
      next: map => { this.topicMap = map; },
      error: () => { /* silently fail — onExplore falls back to generic nav */ }
    });
  }

  onExplore(topicTitle: string): void {
    const entry = this.topicMap.get(topicTitle.toLowerCase());

    if (this.authService.isLoggedIn()) {
      if (entry) {
        const queryParams = entry.reactionId ? { reactionId: entry.reactionId } : {};
        this.router.navigate(['/visualizations', entry.topicId], { queryParams });
      } else {
        this.router.navigate(['/topics']);
      }
    } else {
      if (entry) {
        const queryParams: Record<string, string | number> = {
          returnUrl: `/visualizations/${entry.topicId}`
        };
        if (entry.reactionId) queryParams['reactionId'] = entry.reactionId;
        this.router.navigate(['/login'], { queryParams });
      } else {
        this.router.navigate(['/login']);
      }
    }
  }
}
