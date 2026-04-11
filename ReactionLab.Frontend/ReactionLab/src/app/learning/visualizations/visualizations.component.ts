import { Component, HostBinding, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { LeftpanelComponent } from './leftpanel/leftpanel.component';
import { MaincanvasComponent } from './maincanvas/maincanvas.component';
import { ApiService, Reaction, ReactionStep } from '../../services/api.service';

@Component({
  selector: 'app-visualizations',
  standalone: true,
  imports: [NavbarComponent, LeftpanelComponent, MaincanvasComponent],
  templateUrl: './visualizations.component.html',
  styleUrl: './visualizations.component.css'
})
export class VisualizationsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private apiService = inject(ApiService);

  reaction: Reaction | null = null;
  currentStepIndex = 0;
  topicId = 0;

  private visitedSteps = new Set<number>();
  private completionTriggered = false;

  @HostBinding('class.sidebar-collapsed')

  get totalSteps(): number {
    return this.reaction?.steps?.length ?? 0;
  }

  get currentStep(): ReactionStep | null {
    return this.reaction?.steps?.[this.currentStepIndex] ?? null;
  }

  ngOnInit(): void {
    this.topicId = Number(this.route.snapshot.paramMap.get('topicId'));
    this.apiService.getTopic(this.topicId).subscribe({
      next: (topic) => {
        if (topic.reactions && topic.reactions.length > 0) {
          this.reaction = topic.reactions[0];
          // Step 0 is visible immediately on load
          this.visitedSteps.add(0);
        }
      }
    });
  }

  onStepChange(index: number): void {
    this.currentStepIndex = index;
    this.visitedSteps.add(index);
    this.checkCompletion();
  }

  private checkCompletion(): void {
    if (this.completionTriggered) return;
    if (this.totalSteps === 0) return;

    // Must be on the last step
    if (this.currentStepIndex !== this.totalSteps - 1) return;

    // Every step 0..N-1 must have been visited
    for (let i = 0; i < this.totalSteps; i++) {
      if (!this.visitedSteps.has(i)) return;
    }

    this.completionTriggered = true;
    this.apiService.updateProgress(this.topicId, 'completed').subscribe();
  }

  onSidebarCollapse(collapsed: boolean): void {}
}
