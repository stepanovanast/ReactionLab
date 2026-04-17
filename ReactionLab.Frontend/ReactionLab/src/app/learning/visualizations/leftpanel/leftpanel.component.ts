import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Reaction, ReactionStep } from '../../../services/api.service';

@Component({
  selector: 'app-leftpanel',
  standalone: true,
  imports: [],
  templateUrl: './leftpanel.component.html',
  styleUrl: './leftpanel.component.css'
})
export class LeftpanelComponent {
  @Input() reaction!: Reaction;
  @Input() currentStepIndex = 0;
  @Output() stepChange = new EventEmitter<number>();

  get currentStep(): ReactionStep | null {
    return this.reaction?.steps?.[this.currentStepIndex] ?? null;
  }

  get totalSteps(): number {
    return this.reaction?.steps?.length ?? 0;
  }

  goToStep(index: number): void {
    this.stepChange.emit(index);
  }

  previousStep(): void {
    if (this.currentStepIndex > 0) {
      this.stepChange.emit(this.currentStepIndex - 1);
    }
  }

  nextStep(): void {
    if (this.currentStepIndex < this.totalSteps - 1) {
      this.stepChange.emit(this.currentStepIndex + 1);
    }
  }
}
