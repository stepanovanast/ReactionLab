import { Component, Input, inject } from '@angular/core';
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

  private authService = inject(AuthService);
  private router = inject(Router);

  get isLearningPage(): boolean {
    const url = this.router.url;
    return url.startsWith('/topics') || url.startsWith('/user') || url.startsWith('/visualizations');
  }

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }
}
