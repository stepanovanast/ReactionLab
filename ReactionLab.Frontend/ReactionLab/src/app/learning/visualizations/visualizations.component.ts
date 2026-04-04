import { Component, HostBinding, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { SidebarService } from '../sidebar/sidebar.service';
import { LeftpanelComponent } from './leftpanel/leftpanel.component';
import { MaincanvasComponent } from './maincanvas/maincanvas.component';
import { ApiService, Reaction, ReactionStep } from '../../services/api.service';

@Component({
  selector: 'app-visualizations',
  standalone: true,
  imports: [NavbarComponent, SidebarComponent, LeftpanelComponent, MaincanvasComponent],
  templateUrl: './visualizations.component.html',
  styleUrl: './visualizations.component.css'
})
export class VisualizationsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private apiService = inject(ApiService);
  private sidebarService = inject(SidebarService);

  reaction: Reaction | null = null;
  currentStepIndex = 0;

  @HostBinding('class.sidebar-collapsed')
  get sidebarCollapsed(): boolean {
    return this.sidebarService.isCollapsed;
  }

  get currentStep(): ReactionStep | null {
    return this.reaction?.steps?.[this.currentStepIndex] ?? null;
  }

  ngOnInit(): void {
    const topicId = Number(this.route.snapshot.paramMap.get('topicId'));
    this.apiService.getTopic(topicId).subscribe({
      next: (topic) => {
        if (topic.reactions && topic.reactions.length > 0) {
          this.reaction = topic.reactions[0];
        }
      }
    });
  }

  onStepChange(index: number): void {
    this.currentStepIndex = index;
  }

  onSidebarCollapse(collapsed: boolean): void {}
}
