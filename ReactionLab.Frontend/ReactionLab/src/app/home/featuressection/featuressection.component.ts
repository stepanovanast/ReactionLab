import { Component } from '@angular/core';

@Component({
  selector: 'app-featuressection',
  standalone: true,
  imports: [],
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
