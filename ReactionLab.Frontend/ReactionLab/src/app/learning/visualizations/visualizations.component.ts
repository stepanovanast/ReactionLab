import { Component, HostBinding, inject, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { LeftpanelComponent } from './leftpanel/leftpanel.component';
import { MaincanvasComponent } from './maincanvas/maincanvas.component';
import { ApiService, Reaction, ReactionStep } from '../../services/api.service';
import { ELEMENT_DATA, DEFAULT_ELEMENT_DATA } from './element-data';

@Component({
  selector: 'app-visualizations',
  standalone: true,
  imports: [NavbarComponent, LeftpanelComponent, MaincanvasComponent],
  templateUrl: './visualizations.component.html',
  styleUrl: './visualizations.component.css',
  host: { class: 'notebook-bg' }
})
export class VisualizationsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private apiService = inject(ApiService);
  private titleService = inject(Title);

  reaction: Reaction | null = null;
  currentStepIndex = 0;
  topicId = 0;

  private visitedSteps = new Set<number>();
  private completionTriggered = false;
  private fullCircuitTriggered = false;
  private curiousMindTriggered = false;

  @HostBinding('class.sidebar-collapsed')

  get totalSteps(): number {
    return this.reaction?.steps?.length ?? 0;
  }

  get currentStep(): ReactionStep | null {
    return this.reaction?.steps?.[this.currentStepIndex] ?? null;
  }

  get uniqueElements(): { element: string; color: string }[] {
    const seen = new Set<string>();
    const result: { element: string; color: string }[] = [];
    for (const step of this.reaction?.steps ?? []) {
      for (const atom of step.atoms ?? []) {
        if (!seen.has(atom.element)) {
          seen.add(atom.element);
          result.push({ element: atom.element, color: (ELEMENT_DATA[atom.element] ?? DEFAULT_ELEMENT_DATA).color });
        }
      }
    }
    return result;
  }

  ngOnInit(): void {
    this.topicId = Number(this.route.snapshot.paramMap.get('topicId'));
    this.apiService.getTopic(this.topicId).subscribe({
      next: (topic) => {
        if (topic.reactions && topic.reactions.length > 0) {
          this.reaction = topic.reactions[0];
          this.titleService.setTitle(`${this.reaction.title} | ReactionLab`);
          this.visitedSteps.add(0);
        }
      }
    });
    this.apiService.getTopics().subscribe({
      next: (topics) => {
        const topic = topics.find(t => t.id === this.topicId);
        if (topic?.status === 'completed' && !this.curiousMindTriggered) {
          this.curiousMindTriggered = true;
          this.apiService.awardCuriousMind().subscribe({
            error: (err) => console.error('awardCuriousMind failed:', err)
          });
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
    if (this.totalSteps === 0) return;

    // Award Full Circuit badge the first time the user reaches the final step
    if (!this.fullCircuitTriggered && this.currentStepIndex === this.totalSteps - 1) {
      this.fullCircuitTriggered = true;
      this.apiService.awardFullCircuit().subscribe({
        error: (err) => console.error('awardFullCircuit failed:', err)
      });
    }

    if (this.completionTriggered) return;

    // Must be on the last step
    if (this.currentStepIndex !== this.totalSteps - 1) return;

    // Every step 0..N-1 must have been visited
    for (let i = 0; i < this.totalSteps; i++) {
      if (!this.visitedSteps.has(i)) return;
    }

    this.completionTriggered = true;
    this.apiService.updateProgress(this.topicId, 'completed').subscribe({
      error: (err) => console.error('updateProgress failed:', err)
    });
  }

  onSidebarCollapse(collapsed: boolean): void {}
}
