import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../shared/toast/toast.service';

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
  private toast = inject(ToastService);

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

  selectAvatar(id: number): void {
    this.avatarId = id;
  }

  onSignup(): void {
    if (!this.name || !this.email || !this.password) {
      this.toast.show('Please fill in all fields!', 'error'); return;
    }
    if (this.name.trim().length < 2) {
      this.toast.show('Name must be at least 2 characters long!', 'error'); return;
    }
    if (this.name.trim().length > 50) {
      this.toast.show('Name must be 50 characters or fewer!', 'error'); return;
    }
    const emailRegex = /^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(this.email)) {
      this.toast.show('Please enter a valid email address!', 'error'); return;
    }
    if (this.password !== this.confirmPassword) {
      this.toast.show('The passwords do not match!', 'error'); return;
    }
    if (this.password.length < 6) {
      this.toast.show('The password must be at least 6 characters long!', 'error'); return;
    }
    if (!/[A-Z]/.test(this.password)) {
      this.toast.show('The password must contain at least one uppercase letter!', 'error'); return;
    }
    if (!/[0-9]/.test(this.password)) {
      this.toast.show('The password must contain at least one number!', 'error'); return;
    }

    this.isLoading = true;

    this.authService.signup(this.name, this.email, this.password, this.avatarId).subscribe({
      next: () => {
        this.router.navigate(['/topics']);
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.show(err.error?.error || 'Signup failed. Please try again.', 'error');
      }
    });
  }
}
