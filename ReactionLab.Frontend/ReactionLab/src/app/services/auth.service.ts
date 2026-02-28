import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

// Response types from our backend
interface AuthResponse {
  token: string;
  user: User;
}

export interface User {
  id: number;
  name: string;
  email: string;
}

@Injectable({
  providedIn: 'root'  // Available throughout the app
})
export class AuthService {
  // Backend API URL
  private apiUrl = 'http://localhost:5177/api/auth';

  // BehaviorSubject holds the current user and emits to subscribers
  private currentUserSubject = new BehaviorSubject<User | null>(null);

  // Observable that components can subscribe to
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // On app start, check if user was previously logged in
    this.loadUserFromStorage();
  }

  // =====================================================
  // SIGNUP
  // =====================================================
  signup(name: string, email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/signup`, {
      name,
      email,
      password
    }).pipe(
      tap(response => this.handleAuthResponse(response))
    );
  }

  // =====================================================
  // LOGIN
  // =====================================================
  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, {
      email,
      password
    }).pipe(
      tap(response => this.handleAuthResponse(response))
    );
  }

  // =====================================================
  // LOGOUT
  // =====================================================
  logout(): void {
    // Clear storage
    localStorage.removeItem('token');
    localStorage.removeItem('user');

    // Clear current user
    this.currentUserSubject.next(null);

    // Navigate to home
    this.router.navigate(['/']);
  }

  // =====================================================
  // HELPERS
  // =====================================================

  // Store token and user after successful auth
  private handleAuthResponse(response: AuthResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(response.user));
    this.currentUserSubject.next(response.user);
  }

  // Load user from storage on app start
  private loadUserFromStorage(): void {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      const user = JSON.parse(userJson);
      this.currentUserSubject.next(user);
    }
  }

  // Get the stored JWT token
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // Check if user is logged in
  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  // Get current user synchronously
  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }
}
