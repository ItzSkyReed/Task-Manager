import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import {
  ProjectDto,
  TaskItemStatus,
  TaskItemSummaryDto
} from '../../core/models/api.models';
import { TasksService } from '../../core/services/tasks.service';
import { ProjectsService } from '../../core/services/projects.service';
import { TaskCreateDialogComponent } from './task-create-dialog.component';
import { TaskDetailsDialogComponent } from './task-details-dialog.component';
import { TASK_STATUS_LABELS, TASK_STATUS_OPTIONS } from './task-status.constants';

// @ts-ignore
// @ts-ignore
@Component({
  selector: 'app-tasks-page',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatProgressBarModule
  ],
  template: `
    <div class="page-container">
      <!-- Поиска, фильтры -->
      <div class="header-actions">
        <div class="filters-group">
          <mat-form-field appearance="outline" class="search-field">
            <mat-label>Поиск по названию задачи</mat-label>
            <input matInput [formControl]="searchControl" placeholder="Введите название...">
            <mat-icon matSuffix>search</mat-icon>
          </mat-form-field>

          <mat-form-field appearance="outline" class="filter-field">
            <mat-label>Статус</mat-label>
            <mat-select [formControl]="statusControl">
              <mat-option [value]="null">Все статусы</mat-option>
              @for (option of statusOptions; track option.value) {
                <mat-option [value]="option.value">{{ option.label }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline" class="filter-field">
            <mat-label>Проект</mat-label>
            <mat-select [formControl]="projectControl">
              <mat-option [value]="null">Все проекты</mat-option>
              @for (project of projects(); track project.id) {
                <mat-option [value]="project.id">{{ project.name }}</mat-option>
              }
            </mat-select>
          </mat-form-field>
        </div>

        <button mat-flat-button color="primary" (click)="openCreateDialog()">
          <mat-icon>add_task</mat-icon>
          Создать задачу
        </button>
      </div>

      @if (isLoading()) {
        <mat-progress-bar mode="indeterminate"></mat-progress-bar>
      }

      <!-- Таблица задач -->
      <div class="table-wrapper mat-elevation-z2">
        <table mat-table [dataSource]="tasks()">
          <!-- Название -->
          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef>Название</th>
            <td mat-cell *matCellDef="let task">
              <strong>{{ task.name }}</strong>
            </td>
          </ng-container>

          <!-- Проект -->
          <ng-container matColumnDef="projectName">
            <th mat-header-cell *matHeaderCellDef>Проект</th>
            <td mat-cell *matCellDef="let task">{{ task.projectName }}</td>
          </ng-container>

          <!-- Статус -->
          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Статус</th>
            <td mat-cell *matCellDef="let task">
              <span class="status-badge" [ngClass]="'status-' + task.status">
                {{ getStatusLabel(task.status) }}
              </span>
            </td>
          </ng-container>

          <!-- Исполнитель -->
          <ng-container matColumnDef="assigneeName">
            <th mat-header-cell *matHeaderCellDef>Исполнитель</th>
            <td mat-cell *matCellDef="let task">
              <span [class.unassigned]="!task.assigneeName">
                {{ task.assigneeName || 'Не назначен' }}
              </span>
            </td>
          </ng-container>

          <!-- Дата создания -->
          <ng-container matColumnDef="createdAt">
            <th mat-header-cell *matHeaderCellDef>Создана</th>
            <td mat-cell *matCellDef="let task">
              {{ task.createdAt | date:'dd.MM.yyyy HH:mm' }}
            </td>
          </ng-container>

          <!-- Действия -->
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-header">Действия</th>
            <td mat-cell *matCellDef="let task" class="actions-cell">
              <button mat-icon-button
                      color="primary"
                      title="Подробнее / Редактировать"
                      (click)="openDetailsDialog(task, $event)">
                <mat-icon>visibility</mat-icon>
              </button>
              <button mat-icon-button
                      color="warn"
                      title="Удалить задачу"
                      (click)="deleteTask(task, $event)">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <!-- Клик по строке также открывает подробную карточку задачи -->
          <tr mat-row
              *matRowDef="let row; columns: displayedColumns;"
              class="clickable-row"
              (click)="openDetailsDialog(row)">
          </tr>

          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell empty-cell" [attr.colspan]="displayedColumns.length">
              {{ isLoading() ? 'Загрузка...' : 'Задачи не найдены' }}
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
    .filters-group {
      display: flex;
      gap: 12px;
      flex-wrap: wrap;
      align-items: center;
    }
    .search-field {
      width: 280px;
      margin-bottom: -1.25em;
    }
    .filter-field {
      width: 200px;
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
    .clickable-row {
      cursor: pointer;
    }
    .clickable-row:hover {
      background-color: #f5f7fa;
    }
    .status-badge {
      display: inline-block;
      padding: 4px 10px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
    }
    .status-ToDo {
      background-color: #eceff1;
      color: #455a64;
    }
    .status-InProgress {
      background-color: #e3f2fd;
      color: #1565c0;
    }
    .status-OnReview {
      background-color: #fff3e0;
      color: #e65100;
    }
    .status-Done {
      background-color: #e8f5e9;
      color: #2e7d32;
    }
    .unassigned {
      color: #999;
      font-style: italic;
    }
    .actions-header, .actions-cell {
      text-align: right;
      width: 120px;
    }
    .empty-cell {
      text-align: center;
      padding: 24px;
      color: #777;
    }
  `]
})
export class TasksPageComponent implements OnInit {
  private tasksService = inject(TasksService);
  private projectsService = inject(ProjectsService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private destroyRef = inject(DestroyRef);

  public readonly statusOptions = TASK_STATUS_OPTIONS;
  public readonly statusLabels = TASK_STATUS_LABELS;

  public tasks = signal<TaskItemSummaryDto[]>([]);
  public projects = signal<ProjectDto[]>([]);
  public totalCount = signal<number>(0);
  public pageIndex = signal<number>(0);
  public pageSize = signal<number>(10);
  public isLoading = signal<boolean>(false);

  public searchControl = new FormControl<string>('', { nonNullable: true });
  public statusControl = new FormControl<TaskItemStatus | null>(null);
  public projectControl = new FormControl<string | null>(null);

  public displayedColumns: string[] = [
    'name',
    'projectName',
    'status',
    'assigneeName',
    'createdAt',
    'actions'
  ];

  ngOnInit(): void {

    this.projectsService.getProjects('', 1, 50).subscribe({
      next: (res) => this.projects.set(res.items)
    });

    // Поиск по названию с задержкой 300 мс
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadTasks();
      });


    this.statusControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadTasks();
      });

    this.projectControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadTasks();
      });

    this.loadTasks();
  }

  // Загрузка списка задач с поиском
  public loadTasks(): void {
    this.isLoading.set(true);

    this.tasksService
      .getTasks({
        search: this.searchControl.value,
        status: this.statusControl.value,
        projectId: this.projectControl.value,
        page: this.pageIndex() + 1,
        pageSize: this.pageSize()
      })
      .subscribe({
        next: (response) => {
          this.tasks.set(response.items);
          this.totalCount.set(response.totalCount);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
  }

  public onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadTasks();
  }

  // Открытие окна создания задачи
  public openCreateDialog(): void {
    const dialogRef = this.dialog.open(TaskCreateDialogComponent, {
      width: '560px'
    });

    dialogRef.afterClosed().subscribe((isCreated: boolean) => {
      if (isCreated) {
        this.snackBar.open('Задача успешно создана', 'ОК', { duration: 3000 });
        this.loadTasks();
      }
    });
  }

  // Открытие окна с информацией о задаче
  public openDetailsDialog(task: TaskItemSummaryDto, event?: MouseEvent): void {

    event?.stopPropagation();

    const dialogRef = this.dialog.open(TaskDetailsDialogComponent, {
      data: { taskId: task.id },
      width: '620px'
    });

    dialogRef.afterClosed().subscribe((closedWithChanges: boolean) => {

      if (closedWithChanges || dialogRef.componentInstance.hasChanges) {
        this.loadTasks();
      }
    });
  }

  // Удаление задачи
  public deleteTask(task: TaskItemSummaryDto, event: MouseEvent): void {
    event.stopPropagation();

    if (!confirm(`Удалить задачу «${task.name}»?`)) return;

    this.tasksService.deleteTask(task.id).subscribe({
      next: () => {
        this.snackBar.open('Задача удалена', 'ОК', { duration: 3000 });
        this.loadTasks();
      }
    });
  }
  public getStatusLabel(status: TaskItemStatus): string {
    return this.statusLabels[status] ?? status;
  }
}
