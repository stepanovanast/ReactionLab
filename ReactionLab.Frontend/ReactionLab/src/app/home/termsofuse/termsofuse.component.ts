import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../app/navbar/navbar.component';

@Component({
  selector: 'app-termsofuse',
  standalone: true,
  imports: [RouterLink, NavbarComponent],
  templateUrl: './termsofuse.component.html',
  styleUrl: './termsofuse.component.css'
})
export class TermsofuseComponent {
  scrollTo(id: string): void {
    document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
  }
}
