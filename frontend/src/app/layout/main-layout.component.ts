import { Component, OnInit, inject, ChangeDetectionStrategy } from '@angular/core';

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
      <!-- Верхняя панель управления -->
      <mat-toolbar color="primary" class="toolbar">
        <span class="app-title">TaskManager</span>

        <div class="spacer"></div>

        @if (authService.currentUser(); as user) {
          <span class="user-info">{{ user.name }} ({{ user.email }})</span>
        }

        <button mat-stroked-button (click)="onLogout()" class="logout-btn">
          Выйти
        </button>
      </mat-toolbar>

      <!-- Панель вкладок с обязательной привязкой [tabPanel] -->
      <nav mat-tab-nav-bar [tabPanel]="tabPanel" backgroundColor="primary">
        @for (link of navLinks; track link.path) {
          <a mat-tab-link
             [routerLink]="link.path"
             routerLinkActive #rla="routerLinkActive"
             [active]="rla.isActive">
            {{ link.label }}
          </a>
        }
      </nav>

      <!-- Панель содержимого вкладок -->
      <mat-tab-nav-panel #tabPanel class="tab-panel">
        <main class="content">
          <router-outlet></router-outlet>
        </main>
      </mat-tab-nav-panel>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.Eager,
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
      border-color: rgba(255, 255, 255, 0.5);
    }
    .tab-panel {
      flex: 1;
      overflow-y: auto;
      background-color: #fafafa;
    }
    .content {
      padding: 24px;
    }
  `]
})
export class MainLayoutComponent implements OnInit {
  public authService = inject(AuthService);

  navLinks = [
    { path: '/projects', label: 'Проекты' },
    { path: '/tasks', label: 'Задачи' },
    { path: '/users', label: 'Пользователи' }
  ];

  ngOnInit(): void {
    // Если токен есть в localStorage, а данные пользователя в памяти пусты, подгружаем профиль
    if (this.authService.getToken() && !this.authService.currentUser()) {
      this.authService.getMe().subscribe();
    }
  }

  onLogout(): void {
    this.authService.logout().subscribe({
      error: () => {
        this.authService.logoutLocally();
      }
    });
  }
}
