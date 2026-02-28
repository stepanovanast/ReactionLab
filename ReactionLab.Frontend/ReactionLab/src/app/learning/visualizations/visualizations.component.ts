import { Component, HostBinding, inject } from '@angular/core';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { SidebarService } from '../sidebar/sidebar.service';
import { LeftpanelComponent } from './leftpanel/leftpanel.component';
import { MaincanvasComponent } from './maincanvas/maincanvas.component';

@Component({
  selector: 'app-visualizations',
  standalone: true,
  imports: [NavbarComponent, SidebarComponent, LeftpanelComponent, MaincanvasComponent],
  templateUrl: './visualizations.component.html',
  styleUrl: './visualizations.component.css'
})
export class VisualizationsComponent {
  private sidebarService = inject(SidebarService);

  @HostBinding('class.sidebar-collapsed')
  get sidebarCollapsed(): boolean {
    return this.sidebarService.isCollapsed;
  }

  onSidebarCollapse(collapsed: boolean): void {
    // State is managed by service, this just triggers change detection
  }
}
