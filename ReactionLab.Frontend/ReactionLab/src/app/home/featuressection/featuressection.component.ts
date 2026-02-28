import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-featuressection',
  standalone: true,
  // CommonModule was missing: needed for *ngFor and [innerHTML] in the template
  imports: [CommonModule],
  templateUrl: './featuressection.component.html',
  styleUrl: './featuressection.component.css'
})
export class FeaturessectionComponent {
  features = [
    {
      icon: '<svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/></svg>',
      title: 'Fast Performance',
      description: 'Lightning-fast load times and optimized for performance'
    },
    {
      icon: '<svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="11" width="18" height="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/></svg>',
      title: 'Secure',
      description: 'Enterprise-grade security to keep your data safe'
    },
    {
      icon: '<svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>',
      title: 'Reliable',
      description: '99.9% uptime guarantee with 24/7 monitoring'
    }
  ];
}
