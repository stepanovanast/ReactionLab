import { Component, inject } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

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

  // Form fields (bound with [(ngModel)])
  email = '';
  password = '';

  // UI state
  isLoading = false;
  errorMessage = '';

  onSignIn(): void {
    // Clear previous errors
    this.errorMessage = '';

    // Basic validation
    if (!this.email || !this.password) {
      this.errorMessage = 'Type in you email and password!';
      return;
    }

    // Show loading state
    this.isLoading = true;

    // Call backend API
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
        // Show error message
        this.isLoading = false;
        this.errorMessage = err.error?.error || 'Login failed. Please try again!';
      }
    });
  }
}
