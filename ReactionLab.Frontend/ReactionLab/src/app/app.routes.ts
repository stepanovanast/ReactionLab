import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { TopicsComponent } from './learning/topics/topics.component';
import { VisualizationsComponent } from './learning/visualizations/visualizations.component';
import { TermsofuseComponent } from './home/termsofuse/termsofuse.component';
import { PrivacypolicyComponent } from './home/privacypolicy/privacypolicy.component';
import { DocsComponent } from './home/docs/docs.component';
import { TutorialsComponent } from './home/tutorials/tutorials.component';
import { FaqComponent } from './home/faq/faq.component';
import { ContactsComponent } from './home/contacts/contacts.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  { path: 'signup', component: SignupComponent, canActivate: [guestGuard] },
  { path: 'topics', component: TopicsComponent, canActivate: [authGuard] },
  { path: 'visualizations/:topicId', component: VisualizationsComponent, canActivate: [authGuard] },
  { path: 'terms-of-use', component: TermsofuseComponent },
  { path: 'privacy-policy', component: PrivacypolicyComponent },
  { path: 'documentation', component: DocsComponent },
  { path: 'tutorials', component: TutorialsComponent },
  { path: 'faq', component: FaqComponent },
  { path: 'contacts', component: ContactsComponent },
];
