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
  avatarId: number;
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
  signup(name: string, email: string, password: string, avatarId: number = 1): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/signup`, {
      name,
      email,
      password,
      avatarId
    }).pipe(
      tap(response => this.handleAuthResponse(response, false))
    );
  }

  // =====================================================
  // LOGIN
  // =====================================================
  login(email: string, password: string, rememberMe: boolean = false): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, {
      email,
      password
    }).pipe(
      tap(response => this.handleAuthResponse(response, rememberMe))
    );
  }

  // =====================================================
  // LOGOUT
  // =====================================================
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('user');

    this.currentUserSubject.next(null);
    this.router.navigate(['/']);
  }

  // =====================================================
  // HELPERS
  // =====================================================

  // Store token and user — localStorage if rememberMe, sessionStorage if not
  private handleAuthResponse(response: AuthResponse, rememberMe: boolean): void {
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem('token', response.token);
    storage.setItem('user', JSON.stringify(response.user));
    this.currentUserSubject.next(response.user);
  }

  // Load user from storage on app start (check both)
  private loadUserFromStorage(): void {
    const userJson = localStorage.getItem('user') ?? sessionStorage.getItem('user');
    if (userJson) {
      const user = JSON.parse(userJson);
      this.currentUserSubject.next(user);
    }
  }

  // Get the stored JWT token (check both)
  getToken(): string | null {
    return localStorage.getItem('token') ?? sessionStorage.getItem('token');
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
