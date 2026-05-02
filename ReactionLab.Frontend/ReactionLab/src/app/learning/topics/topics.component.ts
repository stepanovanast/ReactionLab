import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { ApiService, Topic, Badge } from '../../services/api.service';
import { TiltDirective } from '../../shared/tilt.directive';
import { ConfettiService } from '../../shared/confetti.service';


@Component({
  selector: 'app-topics',
  standalone: true,
  imports: [NavbarComponent, TiltDirective],
  templateUrl: './topics.component.html',
  styleUrl: './topics.component.css'
})
export class TopicsComponent implements OnInit {
  private router = inject(Router);
  private apiService = inject(ApiService);
  private confetti = inject(ConfettiService);

  activeFilter: 'all' | 'available' | 'completed' = 'all';
  filterChanging = false;
  private filterTimer?: ReturnType<typeof setTimeout>;
  topics: Topic[] = [];
  badges: Badge[] = [];
  isLoading = true;
  error: string | null = null;
  ringsAnimated = false;

  animatedCompletedCount = 0;
  animatedBadgeCount = 0;

  readonly circumference = 2 * Math.PI * 40;

  private readonly CONFETTI_KEY = 'rl_confetti_badge_count';

  ngOnInit(): void {
    this.apiService.getTopics().subscribe({
      next: (topics) => {
        this.topics = topics;
        this.isLoading = false;
        setTimeout(() => {
          this.ringsAnimated = true;
          this.animateCounter('animatedCompletedCount', this.completedCount);
        }, 50);
      },
      error: () => {
        this.error = 'Failed to load topics. Please try again.';
        this.isLoading = false;
      }
    });

    const lastConfettiCount = Number(localStorage.getItem(this.CONFETTI_KEY) ?? 0);

    this.apiService.getUserBadges().subscribe({
      next: (badges) => {
        this.badges = badges;
        this.animateCounter('animatedBadgeCount', this.earnedBadgesCount);
        if (this.earnedBadgesCount > lastConfettiCount) {
          localStorage.setItem(this.CONFETTI_KEY, String(this.earnedBadgesCount));
          setTimeout(() => this.confetti.burst(), 600);
        }
      },
      error: () => {
        this.badges = [];
      }
    });
  }

  setFilter(filter: 'all' | 'available' | 'completed'): void {
    if (this.activeFilter === filter) return;
    this.filterChanging = true;
    clearTimeout(this.filterTimer);
    this.filterTimer = setTimeout(() => {
      this.activeFilter = filter;
      this.filterChanging = false;
    }, 160);
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
    if (!this.ringsAnimated || !this.topics.length) return this.circumference;
    return this.circumference * (1 - this.completedCount / this.topics.length);
  }

  get badgeProgressOffset(): number {
    if (!this.ringsAnimated || !this.badges.length) return this.circumference;
    return this.circumference * (1 - this.earnedBadgesCount / this.badges.length);
  }

  get nextTopic(): Topic | null {
    return this.topics.find(t => t.status === 'current')
      ?? this.topics.find(t => t.status === 'available')
      ?? null;
  }

  get badgesRow1(): Badge[] { return this.badges.slice(0, 4); }
  get badgesRow2(): Badge[] { return this.badges.slice(4, 8); }

  flippedBadgeIds = new Set<number>();

  toggleBadgeFlip(id: number): void {
    if (this.flippedBadgeIds.has(id)) {
      this.flippedBadgeIds.delete(id);
    } else {
      this.flippedBadgeIds.add(id);
    }
  }

  private animateCounter(key: 'animatedCompletedCount' | 'animatedBadgeCount', target: number): void {
    if (target === 0) return;
    const duration = 900;
    const start = performance.now();
    const step = (now: number) => {
      const progress = Math.min((now - start) / duration, 1);
      const eased = 1 - Math.pow(1 - progress, 3);
      this[key] = Math.round(eased * target);
      if (progress < 1) requestAnimationFrame(step);
    };
    requestAnimationFrame(step);
  }

  onTopicClick(topic: Topic): void {
    this.router.navigate(['/visualizations', topic.id]);
  }
}
