import { Component, ElementRef, HostListener, Input, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  @Input() showAuthButtons = true;
  menuOpen = false;
  profileDropdownOpen = false;

  private authService = inject(AuthService);
  private router = inject(Router);
  private elementRef = inject(ElementRef);

  private readonly avatarMap: Record<number, string> = {
    1: '🧪', 2: '⚗️', 3: '🔬', 4: '🧬', 5: '⚡',
  };

  get isLearningPage(): boolean {
    const url = this.router.url;
    return url.startsWith('/topics') || url.startsWith('/user') || url.startsWith('/visualizations');
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
