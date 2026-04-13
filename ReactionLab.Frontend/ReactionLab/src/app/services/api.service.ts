import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject, forkJoin, of } from 'rxjs';
import { switchMap, map } from 'rxjs/operators';
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

export interface Atom {
  id: string;
  element: string;
  x: number;
  y: number;
  z: number;
}

export interface Bond {
  from: string;
  to: string;
}

export interface ReactionStep {
  number: number;
  title: string;
  description: string;
  atoms: Atom[];
  bonds: Bond[];
  temperatureRange: { start: number; end: number };
  background: 'default' | 'glow' | 'dark';
  electronTransfers: { from: string; to: string }[];
  atomOverrides: Record<string, { radiusScale?: number; dimmed?: boolean; glowing?: boolean; darkened?: boolean }>;
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

  /** Emit here whenever a badge may have been awarded so the navbar re-checks. */
  readonly badgeRefresh$ = new Subject<void>();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  // =====================================================
  // TOPICS (Public endpoints)
  // =====================================================

  // Get all topics — sends auth header when logged in so the backend returns personalized statuses
  getTopics(search?: string): Observable<Topic[]> {
    const url = search
      ? `${this.apiUrl}/topics?search=${encodeURIComponent(search)}`
      : `${this.apiUrl}/topics`;
    const token = this.authService.getToken();
    const options = token ? { headers: this.getAuthHeaders() } : {};
    return this.http.get<Topic[]>(url, options);
  }

  // Get single topic with reactions
  getTopic(id: number): Observable<TopicDetail> {
    return this.http.get<TopicDetail>(`${this.apiUrl}/topics/${id}`);
  }

  // Fetch all topics keyed by lowercase title, with first reactionId if one exists
  getTopicMap(): Observable<Map<string, { topicId: number; reactionId?: number }>> {
    return this.getTopics().pipe(
      switchMap(topics => {
        if (topics.length === 0) return of(new Map());
        return forkJoin(topics.map(t => this.getTopic(t.id))).pipe(
          map(details => {
            const m = new Map<string, { topicId: number; reactionId?: number }>();
            details.forEach(td => {
              m.set(td.title.toLowerCase(), {
                topicId: td.id,
                reactionId: td.reactions?.[0]?.id
              });
            });
            return m;
          })
        );
      })
    );
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

  // Award Molecular Vision badge (first visualization mode switch)
  awardMolecularVision(): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/user/badges/molecular-vision`,
      {},
      { headers: this.getAuthHeaders() }
    );
  }

  // Award Full Circuit badge (reached final step of any reaction)
  awardFullCircuit(): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/user/badges/full-circuit`,
      {},
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
