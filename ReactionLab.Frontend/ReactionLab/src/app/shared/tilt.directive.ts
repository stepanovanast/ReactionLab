import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[appTilt]',
  standalone: true
})
export class TiltDirective {
  @Input() tiltMax = 8;
  @Input() tiltLift = '-2px';

  constructor(private el: ElementRef) {}

  @HostListener('mousemove', ['$event'])
  onMouseMove(e: MouseEvent): void {
    const rect = this.el.nativeElement.getBoundingClientRect();
    const x = (e.clientX - rect.left) / rect.width - 0.5;
    const y = (e.clientY - rect.top) / rect.height - 0.5;
    this.el.nativeElement.style.transform =
      `perspective(700px) rotateX(${(-y * this.tiltMax).toFixed(2)}deg) rotateY(${(x * this.tiltMax).toFixed(2)}deg) translateY(${this.tiltLift})`;
    this.el.nativeElement.style.transition = 'transform 0.05s ease';
  }

  @HostListener('mouseleave')
  onMouseLeave(): void {
    this.el.nativeElement.style.transform = '';
    this.el.nativeElement.style.transition = 'transform 0.35s ease';
  }
}
