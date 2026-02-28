import { Component, HostBinding, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../app/navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { SidebarService } from '../sidebar/sidebar.service';

interface Topic {
  id: number;
  title: string;
  status: 'current' | 'completed' | 'available' | 'locked';
}

@Component({
  selector: 'app-topics',
  standalone: true,
  imports: [FormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './topics.component.html',
  styleUrl: './topics.component.css'
})
export class TopicsComponent {
  private router = inject(Router);
  private sidebarService = inject(SidebarService);
  searchQuery = '';

  @HostBinding('class.sidebar-collapsed')
  get sidebarCollapsed(): boolean {
    return this.sidebarService.isCollapsed;
  }

  onSidebarCollapse(collapsed: boolean): void {
    // State is managed by service, this just triggers change detection
  }

  topics: Topic[] = [
    { id: 1, title: 'Topic 1', status: 'current' },
    { id: 2, title: 'Topic 2', status: 'completed' },
    { id: 3, title: 'Topic 3', status: 'available' },
    { id: 4, title: 'Topic 4', status: 'available' },
    { id: 5, title: 'Topic 5', status: 'available' },
    { id: 6, title: 'Topic 6', status: 'available' },
  ];

  get filteredTopics(): Topic[] {
    return this.topics.filter(topic =>
      topic.title.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  onTopicClick(topic: Topic): void {
    this.router.navigate(['/visualizations', topic.id]);
  }
}
