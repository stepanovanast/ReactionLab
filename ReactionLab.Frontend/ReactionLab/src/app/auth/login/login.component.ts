import { Component, inject } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../shared/toast/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);
  private toast = inject(ToastService);

  email = '';
  password = '';
  isLoading = false;

  onSignIn(): void {
    if (!this.email || !this.password) {
      this.toast.show('Please enter your email and password!', 'error');
      return;
    }

    this.isLoading = true;

    this.authService.login(this.email, this.password, false).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'];
        const reactionId = this.route.snapshot.queryParams['reactionId'];
        if (returnUrl) {
          this.router.navigateByUrl(returnUrl + (reactionId ? `?reactionId=${reactionId}` : ''));
        } else {
          this.router.navigate(['/topics']);
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.show(err.error?.error || 'Login failed. Please try again!', 'error');
      }
    });
  }
}
