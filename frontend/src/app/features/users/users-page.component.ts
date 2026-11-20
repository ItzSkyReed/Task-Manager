import { Component, DestroyRef, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { UserDto } from '../../core/models/api.models';
import { UsersService } from '../../core/services/users.service';
import { AuthService } from '../../core/services/auth.service';
import { UserCreateDialogComponent } from './user-create-dialog.component';

@Component({
  selector: 'app-users-page',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatProgressBarModule
  ],
  template: `
    <div class="page-container">
      <!-- Панель поиска и кнопка добавления -->
      <div class="header-actions">
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Поиск по имени пользователя</mat-label>
          <input matInput [formControl]="searchControl" placeholder="Введите имя...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        <button mat-flat-button color="primary" (click)="openCreateDialog()">
          <mat-icon>person_add</mat-icon>
          Добавить пользователя
        </button>
      </div>

      <!-- Индикатор загрузки -->
      @if (isLoading()) {
        <mat-progress-bar mode="indeterminate"></mat-progress-bar>
      }

      <!-- Таблица пользователей -->
      <div class="table-wrapper mat-elevation-z2">
        <table mat-table [dataSource]="users()">
          <!-- Колонка: Имя -->
          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef>Имя</th>
            <td mat-cell *matCellDef="let user">
              <strong>{{ user.name }}</strong>
              @if (authService.currentUser()?.id === user.id) {
                <span class="current-user-badge">Вы</span>
              }
            </td>
          </ng-container>

          <!-- Колонка: Email -->
          <ng-container matColumnDef="email">
            <th mat-header-cell *matHeaderCellDef>Email</th>
            <td mat-cell *matCellDef="let user">{{ user.email }}</td>
          </ng-container>

          <!-- Колонка: Дата создания -->
          <ng-container matColumnDef="createdAt">
            <th mat-header-cell *matHeaderCellDef>Дата регистрации</th>
            <td mat-cell *matCellDef="let user">
              {{ user.createdAt | date:'dd.MM.yyyy HH:mm' }}
            </td>
          </ng-container>

          <!-- Колонка: Действия -->
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-header">Действия</th>
            <td mat-cell *matCellDef="let user" class="actions-cell">
              <button mat-icon-button
                      color="warn"
                      title="Удалить пользователя"
                      (click)="deleteUser(user)">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>

          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell empty-cell" [attr.colspan]="displayedColumns.length">
              {{ isLoading() ? 'Загрузка...' : 'Пользователи не найдены' }}
            </td>
          </tr>
        </table>

        <!-- Серверная пагинация -->
        <mat-paginator [length]="totalCount()"
                       [pageIndex]="pageIndex()"
                       [pageSize]="pageSize()"
                       [pageSizeOptions]="[5, 10, 25, 50]"
                       (page)="onPageChange($event)"
                       showFirstLastButtons>
        </mat-paginator>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.Eager,
  styles: [`
    .page-container {
      max-width: 1200px;
      margin: 0 auto;
    }
    .header-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 16px;
      margin-bottom: 16px;
      flex-wrap: wrap;
    }
    .search-field {
      width: 350px;
      margin-bottom: -1.25em;
    }
    .table-wrapper {
      background: white;
      border-radius: 4px;
      overflow: hidden;
    }
    table {
      width: 100%;
    }
    .current-user-badge {
      margin-left: 8px;
      padding: 2px 8px;
      border-radius: 12px;
      font-size: 12px;
      background-color: #e3f2fd;
      color: #1565c0;
      font-weight: 500;
    }
    .actions-header, .actions-cell {
      text-align: right;
      width: 100px;
    }
    .empty-cell {
      text-align: center;
      padding: 24px;
      color: #777;
    }
  `]
})
export class UsersPageComponent implements OnInit {
  private usersService = inject(UsersService);
  public authService = inject(AuthService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private destroyRef = inject(DestroyRef);

  public users = signal<UserDto[]>([]);
  public totalCount = signal<number>(0);
  public pageIndex = signal<number>(0);
  public pageSize = signal<number>(10);
  public isLoading = signal<boolean>(false);

  public searchControl = new FormControl<string>('', { nonNullable: true });
  public displayedColumns: string[] = ['name', 'email', 'createdAt', 'actions'];

  ngOnInit(): void {
    // Реактивный поиск по имени с задержкой 300 мс
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadUsers();
      });

    this.loadUsers();
  }

  // Загрузка списка пользователей с бэкенда
  public loadUsers(): void {
    this.isLoading.set(true);

    this.usersService
      .getUsers(this.searchControl.value, this.pageIndex() + 1, this.pageSize())
      .subscribe({
        next: (response) => {
          this.users.set(response.items);
          this.totalCount.set(response.totalCount);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
  }

  // Переключение страницы или размера страницы
  public onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadUsers();
  }

  // Открытие окна создания нового пользователя
  public openCreateDialog(): void {
    const dialogRef = this.dialog.open(UserCreateDialogComponent, {
      width: '480px'
    });

    dialogRef.afterClosed().subscribe((isCreated: boolean) => {
      if (isCreated) {
        this.snackBar.open('Пользователь успешно добавлен', 'ОК', { duration: 3000 });
        this.loadUsers();
      }
    });
  }

  // Удаление пользователя с подтверждением
  public deleteUser(user: UserDto): void {
    const isSelf = this.authService.currentUser()?.id === user.id;
    const message = isSelf
      ? 'Вы уверены, что хотите удалить СВОЙ собственный аккаунт? Вы будете разлогинены.'
      : `Удалить пользователя «${user.name}» (${user.email})?`;

    if (!confirm(message)) return;

    this.usersService.deleteUser(user.id).subscribe({
      next: () => {
        if (isSelf) {
          this.snackBar.open('Ваш аккаунт удален', 'ОК', { duration: 3000 });
          this.authService.logoutLocally();
          return;
        }

        this.snackBar.open('Пользователь удален', 'ОК', { duration: 3000 });
        this.loadUsers();
      }
    });
  }
}
