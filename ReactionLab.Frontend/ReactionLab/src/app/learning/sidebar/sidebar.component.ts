import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { SidebarService } from './sidebar.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit {
  private router = inject(Router);
  private sidebarService = inject(SidebarService);

  @Output() collapsedChange = new EventEmitter<boolean>();

  get isCollapsed(): boolean {
    return this.sidebarService.isCollapsed;
  }

  ngOnInit(): void {
    this.collapsedChange.emit(this.sidebarService.isCollapsed);
  }

  toggleCollapse(): void {
    this.sidebarService.toggle();
    this.collapsedChange.emit(this.sidebarService.isCollapsed);
  }

  signOut(): void {
    this.router.navigate(['/']);
  }
}
