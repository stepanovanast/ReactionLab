import { Component } from '@angular/core';
import { NavbarComponent } from '../app/navbar/navbar.component';
import { HerosectionComponent } from './herosection/herosection.component';
import { FeaturessectionComponent } from './featuressection/featuressection.component';
import { ReactionsectionComponent } from './reactionsection/reactionsection.component';
import { LibrarysectionComponent } from './librarysection/librarysection.component';
import { WhysectionComponent } from './whysection/whysection.component';
import { FooterComponent } from './footer/footer.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    NavbarComponent,
    HerosectionComponent,
    FeaturessectionComponent,
    ReactionsectionComponent,
    LibrarysectionComponent,
    WhysectionComponent,
    FooterComponent
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}