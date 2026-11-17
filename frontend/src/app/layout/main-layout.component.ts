import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatTabsModule,
    MatIconModule
  ],
  template: `
    <div class="layout-container">

      <mat-toolbar color="primary" class="toolbar">
        <span class="app-title">TaskManager</span>

        <div class="spacer"></div>

        @if (authService.currentUser(); as user) {
          <span class="user-info">{{ user.name }} ({{ user.email }})</span>
        }

        <button mat-stroked-button color="warn" (click)="onLogout()" class="logout-btn">
          Выйти
        </button>
      </mat-toolbar>


      <nav mat-tab-nav-bar [backgroundColor]="'primary'">
        <a mat-tab-link
           *ngFor="let link of navLinks"
           [routerLink]="link.path"
           routerLinkActive #rla="routerLinkActive"
           [active]="rla.isActive">
          {{ link.label }}
        </a>
      </nav>


      <main class="content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .layout-container {
      display: flex;
      flex-direction: column;
      height: 100vh;
    }
    .spacer {
      flex: 1 1 auto;
    }
    .user-info {
      margin-right: 20px;
      font-size: 14px;
    }
    .logout-btn {
      color: white;
      border-color: rgba(255,255,255,0.5);
    }
    .content {
      flex: 1;
      padding: 24px;
      background-color: #fafafa;
      overflow-y: auto;
    }
  `]
})
export class MainLayoutComponent {
  public authService = inject(AuthService);

  // Ссылки на вкладки
  navLinks = [
    { path: '/projects', label: 'Проекты' },
    { path: '/tasks', label: 'Задачи' },
    { path: '/users', label: 'Пользователи' }
  ];

  onLogout(): void {
    this.authService.logout().subscribe({
      next: () => {},
      error: () => {

        this.authService.logoutLocally(); // выходим даже при ошибке
      }
    });
  }
}
