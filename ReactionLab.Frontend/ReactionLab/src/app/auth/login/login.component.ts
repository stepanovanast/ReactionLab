import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
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
      this.errorMessage = 'Please enter email and password';
      return;
    }

    // Show loading state
    this.isLoading = true;

    // Call backend API
    this.authService.login(this.email, this.password).subscribe({
      next: () => {
        // Success! Navigate to topics page
        this.router.navigate(['/topics']);
      },
      error: (err) => {
        // Show error message
        this.isLoading = false;
        this.errorMessage = err.error?.error || 'Login failed. Please try again.';
      }
    });
  }
}
