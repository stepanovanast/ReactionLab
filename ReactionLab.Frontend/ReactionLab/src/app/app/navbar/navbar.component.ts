import { Component, ElementRef, HostListener, Input, OnInit, OnDestroy, inject, computed } from '@angular/core';
import { Router, RouterLink, NavigationEnd } from '@angular/router';
import { Subscription, filter } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit, OnDestroy {
  @Input() showAuthButtons = true;
  menuOpen = false;
  profileDropdownOpen = false;
  scrolled = false;

  @HostListener('window:scroll')
  onScroll(): void {
    this.scrolled = window.scrollY > 20;
  }

  private authService = inject(AuthService);
  private apiService = inject(ApiService);
  private router = inject(Router);
  private elementRef = inject(ElementRef);

  private readonly BADGE_STORAGE_KEY = 'rl_seen_badge_count';
  private routerSub?: Subscription;

  readonly hasNewBadge = computed(() =>
    this.apiService.earnedBadgeCount() > Number(localStorage.getItem(this.BADGE_STORAGE_KEY) ?? 0)
  );

  private readonly avatarMap: Record<number, string> = {
    1: '🧪', 2: '⚗️', 3: '🔬', 4: '🧬', 5: '⚡',
  };

  get isLearningPage(): boolean {
    const url = this.router.url;
    return url.startsWith('/topics') || url.startsWith('/user') || url.startsWith('/visualizations');
  }

  get isVisualizationsPage(): boolean {
    return this.router.url.startsWith('/visualizations');
  }

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  get userName(): string {
    return this.authService.getCurrentUser()?.name ?? '';
  }

  get avatarEmoji(): string {
    const id = this.authService.getCurrentUser()?.avatarId ?? 1;
    return this.avatarMap[id] ?? '🧪';
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.apiService.getUserBadges().subscribe();
    }
    this.routerSub = this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((e) => {
        const url = (e as NavigationEnd).urlAfterRedirects;
        if (url.startsWith('/topics')) {
          this.clearBadgeAlert();
        }
      });
  }

  ngOnDestroy(): void {
    this.routerSub?.unsubscribe();
  }

  clearBadgeAlert(): void {
    localStorage.setItem(this.BADGE_STORAGE_KEY, String(this.apiService.earnedBadgeCount()));
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  toggleProfileDropdown(): void {
    this.profileDropdownOpen = !this.profileDropdownOpen;
  }

  signOut(): void {
    this.profileDropdownOpen = false;
    this.authService.logout();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.profileDropdownOpen = false;
      this.menuOpen = false;
    }
  }
}
