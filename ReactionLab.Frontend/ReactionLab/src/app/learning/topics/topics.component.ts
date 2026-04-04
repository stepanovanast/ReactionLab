import { Component, HostBinding, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { SidebarService } from '../sidebar/sidebar.service';
import { ApiService, Topic } from '../../services/api.service';

@Component({
  selector: 'app-topics',
  standalone: true,
  imports: [FormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './topics.component.html',
  styleUrl: './topics.component.css'
})
export class TopicsComponent implements OnInit {
  private router = inject(Router);
  private sidebarService = inject(SidebarService);
  private apiService = inject(ApiService);

  searchQuery = '';
  topics: Topic[] = [];
  isLoading = true;
  error: string | null = null;

  @HostBinding('class.sidebar-collapsed')
  get sidebarCollapsed(): boolean {
    return this.sidebarService.isCollapsed;
  }

  ngOnInit(): void {
    this.apiService.getTopics().subscribe({
      next: (topics) => {
        this.topics = topics;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load topics. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onSidebarCollapse(collapsed: boolean): void {}

  get filteredTopics(): Topic[] {
    return this.topics.filter(topic =>
      topic.title.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  onTopicClick(topic: Topic): void {
    this.router.navigate(['/visualizations', topic.id]);
  }
}
