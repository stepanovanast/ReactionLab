import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [RouterLink, FormsModule, CommonModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  private router = inject(Router);
  private authService = inject(AuthService);

  // Form fields
  name = '';
  email = '';
  password = '';
  confirmPassword = '';

  // UI state
  isLoading = false;
  errorMessage = '';

  onSignup(): void {
    // Clear previous errors
    this.errorMessage = '';

    // Validation
    if (!this.name || !this.email || !this.password) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    if (this.password.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters';
      return;
    }

    // Show loading state
    this.isLoading = true;

    // Call backend API
    this.authService.signup(this.name, this.email, this.password).subscribe({
      next: () => {
        // Success! User is logged in automatically, go to topics
        this.router.navigate(['/topics']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.error || 'Signup failed. Please try again.';
      }
    });
  }
}
