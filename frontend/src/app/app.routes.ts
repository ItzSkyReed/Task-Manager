import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { MainLayoutComponent } from './layout/main-layout.component';
import { authGuard } from './core/guards/auth.guard';

import { ProjectsPageComponent } from './features/projects/projects-page.component';
import { TasksPageComponent } from './features/tasks/tasks-page.component';
import { UsersPageComponent } from './features/users/users-page.component';

export const routes: Routes = [
  // Публичные маршруты
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Защищенные маршруты
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'projects', component: ProjectsPageComponent },
      { path: 'tasks', component: TasksPageComponent },
      { path: 'users', component: UsersPageComponent },
      { path: '', redirectTo: 'projects', pathMatch: 'full' } // Редирект по умолчанию
    ]
  },

  // Если путь не найден, то будет редирект на проекты
  { path: '**', redirectTo: '' }
];
