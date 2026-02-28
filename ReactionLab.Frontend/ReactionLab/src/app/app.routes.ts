import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { ForgotComponent } from './auth/forgot/forgot.component';
import { TopicsComponent } from './learning/topics/topics.component';
import { UserComponent } from './learning/user/user.component';
import { VisualizationsComponent } from './learning/visualizations/visualizations.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'forgot', component: ForgotComponent },
  { path: 'topics', component: TopicsComponent },
  { path: 'user', component: UserComponent },
  { path: 'visualizations/:topicId', component: VisualizationsComponent },
];
