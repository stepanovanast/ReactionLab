import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Reaction, ReactionStep } from '../../../services/api.service';

const ELEMENT_COLORS: Record<string, string> = {
  'Fe': '#B8B8B8',
  'S':  '#FFCC00',
  'H':  '#EEEEEE',
  'O':  '#FF4444',
  'C':  '#333333',
  'N':  '#3366FF',
};

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

  get uniqueElements(): { element: string; color: string }[] {
    const seen = new Set<string>();
    const result: { element: string; color: string }[] = [];
    for (const step of this.reaction?.steps ?? []) {
      for (const atom of step.atoms ?? []) {
        if (!seen.has(atom.element)) {
          seen.add(atom.element);
          result.push({ element: atom.element, color: ELEMENT_COLORS[atom.element] ?? '#888888' });
        }
      }
    }
    return result;
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
