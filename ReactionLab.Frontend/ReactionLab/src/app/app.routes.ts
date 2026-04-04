import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { ForgotComponent } from './auth/forgot/forgot.component';
import { TopicsComponent } from './learning/topics/topics.component';
import { UserComponent } from './learning/user/user.component';
import { VisualizationsComponent } from './learning/visualizations/visualizations.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  { path: 'signup', component: SignupComponent, canActivate: [guestGuard] },
  { path: 'forgot', component: ForgotComponent, canActivate: [guestGuard] },
  { path: 'topics', component: TopicsComponent, canActivate: [authGuard] },
  { path: 'user', component: UserComponent, canActivate: [authGuard] },
  { path: 'visualizations/:topicId', component: VisualizationsComponent, canActivate: [authGuard] },
];
