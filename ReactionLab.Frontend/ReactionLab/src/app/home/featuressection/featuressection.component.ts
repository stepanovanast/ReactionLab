import { Component } from '@angular/core';
import { FadeInScrollDirective } from '../../shared/fade-in-scroll.directive';

@Component({
  selector: 'app-featuressection',
  standalone: true,
  imports: [FadeInScrollDirective],
  templateUrl: './featuressection.component.html',
  styleUrl: './featuressection.component.css'
})
export class FeaturessectionComponent {
  highlightBadges = false;
  highlightMolecules = false;
  highlightViewToggle = false;
  highlightTrophy = false;
  highlightPlayback = false;
}
