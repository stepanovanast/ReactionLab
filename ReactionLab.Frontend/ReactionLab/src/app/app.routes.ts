import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { TopicsComponent } from './learning/topics/topics.component';
import { VisualizationsComponent } from './learning/visualizations/visualizations.component';
import { TermsofuseComponent } from './home/termsofuse/termsofuse.component';
import { PrivacypolicyComponent } from './home/privacypolicy/privacypolicy.component';
import { DocsComponent } from './home/docs/docs.component';
import { FaqComponent } from './home/faq/faq.component';
import { ContactsComponent } from './home/contacts/contacts.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';
import { topicAccessGuard } from './guards/topic-access.guard';
import { NotfoundComponent } from './notfound/notfound.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, title: 'ReactionLab' },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard], title: 'Log in | ReactionLab' },
  { path: 'signup', component: SignupComponent, canActivate: [guestGuard], title: 'Sign up | ReactionLab' },
  { path: 'topics', component: TopicsComponent, canActivate: [authGuard], title: 'My dashboard | ReactionLab' },
  { path: 'visualizations/:topicId', component: VisualizationsComponent, canActivate: [authGuard, topicAccessGuard] },
  { path: 'terms-of-use', component: TermsofuseComponent, title: 'Terms of use | ReactionLab' },
  { path: 'privacy-policy', component: PrivacypolicyComponent, title: 'Privacy policy | ReactionLab' },
  { path: 'tutorials', component: DocsComponent, title: 'Tutorials | ReactionLab' },
  { path: 'faq', component: FaqComponent, title: 'FAQ | ReactionLab' },
  { path: 'contacts', component: ContactsComponent, title: 'Contacts | ReactionLab' },
  { path: '**', component: NotfoundComponent, title: '404 | ReactionLab' },
];
