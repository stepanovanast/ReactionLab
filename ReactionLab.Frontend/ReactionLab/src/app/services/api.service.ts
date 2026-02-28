import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

// Types matching our backend responses
export interface Topic {
  id: number;
  title: string;
  description: string;
  status: 'locked' | 'available' | 'current' | 'completed';
}

export interface TopicDetail {
  id: number;
  title: string;
  description: string;
  reactions: Reaction[];
}

export interface Reaction {
  id: number;
  title: string;
  equation: string;
  temperature: number;
  molecules: any[];
  steps: ReactionStep[];
}

export interface ReactionStep {
  number: number;
  title: string;
  description: string;
}

export interface Badge {
  id: number;
  name: string;
  description: string;
  earned: boolean;
}

export interface UserProfile {
  id: number;
  name: string;
  email: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5177/api';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  // =====================================================
  // TOPICS (Public endpoints)
  // =====================================================

  // Get all topics
  getTopics(search?: string): Observable<Topic[]> {
    const url = search
      ? `${this.apiUrl}/topics?search=${encodeURIComponent(search)}`
      : `${this.apiUrl}/topics`;
    return this.http.get<Topic[]>(url);
  }

  // Get single topic with reactions
  getTopic(id: number): Observable<TopicDetail> {
    return this.http.get<TopicDetail>(`${this.apiUrl}/topics/${id}`);
  }

  // =====================================================
  // USER (Protected endpoints - require JWT)
  // =====================================================

  // Get user profile
  getUserProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(
      `${this.apiUrl}/user/profile`,
      { headers: this.getAuthHeaders() }
    );
  }

  // Get user badges
  getUserBadges(): Observable<Badge[]> {
    return this.http.get<Badge[]>(
      `${this.apiUrl}/user/badges`,
      { headers: this.getAuthHeaders() }
    );
  }

  // Update progress on a topic
  updateProgress(topicId: number, status: string): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/user/progress`,
      { topicId, status },
      { headers: this.getAuthHeaders() }
    );
  }

  // =====================================================
  // HELPER: Add JWT token to requests
  // =====================================================
  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }
}
