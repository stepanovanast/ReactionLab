import { Component, HostBinding, OnInit, inject } from '@angular/core';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { SidebarService } from '../sidebar/sidebar.service';
import { AuthService } from '../../services/auth.service';
import { ApiService, Badge } from '../../services/api.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [NavbarComponent, SidebarComponent],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  private sidebarService = inject(SidebarService);
  private authService = inject(AuthService);
  private apiService = inject(ApiService);

  badges: Badge[] = [];

  get userName(): string {
    return this.authService.getCurrentUser()?.name ?? '';
  }

  @HostBinding('class.sidebar-collapsed')
  get sidebarCollapsed(): boolean {
    return this.sidebarService.isCollapsed;
  }

  ngOnInit(): void {
    this.apiService.getUserBadges().subscribe({
      next: (badges) => this.badges = badges,
      error: () => this.badges = []
    });
  }

  onSidebarCollapse(collapsed: boolean): void {}
}
