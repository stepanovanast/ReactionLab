import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-herosection',
  standalone: true,
  imports: [],
  templateUrl: './herosection.component.html',
  styleUrl: './herosection.component.css'
})
export class HerosectionComponent {
  private router = inject(Router);
  private authService = inject(AuthService);

  onStartExploring(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/topics']);
    } else {
      this.router.navigate(['/login']);
    }
  }

  scrollToFeatures(): void {
    document.querySelector('app-featuressection')?.scrollIntoView({ behavior: 'smooth' });
  }
}
