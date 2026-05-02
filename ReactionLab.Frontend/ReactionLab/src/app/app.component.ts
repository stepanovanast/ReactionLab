import { Component, HostListener } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { trigger, transition, style, animate, query } from '@angular/animations';
import { ToastComponent } from './shared/toast/toast.component';

const routeAnimation = trigger('routeAnimation', [
  transition('* <=> *', [
    query(':enter', [
      style({ opacity: 0, transform: 'translateY(8px)' }),
      animate('200ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
    ], { optional: true })
  ])
]);

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ToastComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  animations: [routeAnimation]
})
export class AppComponent {
  title = 'ReactionLab';
  scrollProgress = 0;

  @HostListener('window:scroll')
  onScroll(): void {
    const scrollTop = window.scrollY;
    const docHeight = document.documentElement.scrollHeight - window.innerHeight;
    this.scrollProgress = docHeight > 0 ? (scrollTop / docHeight) * 100 : 0;
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  getRouteState(outlet: RouterOutlet): string {
    return outlet.isActivated ? (outlet.activatedRoute.snapshot.routeConfig?.path ?? '') : '';
  }
}
