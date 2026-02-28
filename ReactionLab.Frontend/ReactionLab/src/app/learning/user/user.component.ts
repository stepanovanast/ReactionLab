import { Component, HostBinding, inject } from '@angular/core';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { SidebarService } from '../sidebar/sidebar.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [NavbarComponent, SidebarComponent],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent {
  private sidebarService = inject(SidebarService);

  @HostBinding('class.sidebar-collapsed')
  get sidebarCollapsed(): boolean {
    return this.sidebarService.isCollapsed;
  }

  onSidebarCollapse(collapsed: boolean): void {
    // State is managed by service, this just triggers change detection
  }
}
