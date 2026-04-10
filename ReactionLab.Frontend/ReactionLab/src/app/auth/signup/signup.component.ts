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

  name = '';
  email = '';
  password = '';
  confirmPassword = '';
  avatarId = 1;

  avatars = [
    { id: 1, emoji: '🧪' },
    { id: 2, emoji: '⚗️' },
    { id: 3, emoji: '🔬' },
    { id: 4, emoji: '🧬' },
    { id: 5, emoji: '⚡' },
  ];

  isLoading = false;
  errorMessage = '';

  selectAvatar(id: number): void {
    this.avatarId = id;
  }

  onSignup(): void {
    this.errorMessage = '';

    if (!this.name || !this.email || !this.password) {
      this.errorMessage = 'Please fill in all fields!';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'The passwords do not match!';
      return;
    }

    if (this.password.length < 6) {
      this.errorMessage = 'The password must be at least 6 characters long!';
      return;
    }

    this.isLoading = true;

    this.authService.signup(this.name, this.email, this.password, this.avatarId).subscribe({
      next: () => {
        this.router.navigate(['/topics']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.error || 'Signup failed. Please try again.';
      }
    });
  }
}
