import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { ApiService, Topic, Badge } from '../../services/api.service';


@Component({
  selector: 'app-topics',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './topics.component.html',
  styleUrl: './topics.component.css'
})
export class TopicsComponent implements OnInit {
  private router = inject(Router);
  private apiService = inject(ApiService);

  activeFilter: 'all' | 'available' | 'completed' = 'all';
  topics: Topic[] = [];
  badges: Badge[] = [];
  isLoading = true;
  error: string | null = null;

  readonly circumference = 2 * Math.PI * 40;

  ngOnInit(): void {
    this.apiService.getTopics().subscribe({
      next: (topics) => {
        this.topics = topics;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load topics. Please try again.';
        this.isLoading = false;
      }
    });

    this.apiService.getUserBadges().subscribe({
      next: (badges) => {
        this.badges = badges;
      },
      error: () => {
        this.badges = [];
      }
    });
  }

  setFilter(filter: 'all' | 'available' | 'completed'): void {
    this.activeFilter = filter;
  }

  get filteredTopics(): Topic[] {
    return this.topics.filter(topic => {
      if (this.activeFilter === 'available') return topic.status === 'available' || topic.status === 'current';
      if (this.activeFilter === 'completed') return topic.status === 'completed';
      return true;
    });
  }

  get completedCount(): number {
    return this.topics.filter(t => t.status === 'completed').length;
  }

  get availableCount(): number {
    return this.topics.filter(t => t.status === 'available' || t.status === 'current').length;
  }

  get lockedCount(): number {
    return this.topics.filter(t => t.status === 'locked').length;
  }

  get earnedBadgesCount(): number {
    return this.badges.filter(b => b.earned).length;
  }

  get progressRingOffset(): number {
    if (!this.topics.length) return this.circumference;
    return this.circumference * (1 - this.completedCount / this.topics.length);
  }

  get badgeProgressOffset(): number {
    if (!this.badges.length) return this.circumference;
    return this.circumference * (1 - this.earnedBadgesCount / this.badges.length);
  }

  get nextTopic(): Topic | null {
    return this.topics.find(t => t.status === 'current')
      ?? this.topics.find(t => t.status === 'available')
      ?? null;
  }

  get badgesRow1(): Badge[] { return this.badges.slice(0, 4); }
  get badgesRow2(): Badge[] { return this.badges.slice(4, 8); }

  onTopicClick(topic: Topic): void {
    this.router.navigate(['/visualizations', topic.id]);
  }
}
