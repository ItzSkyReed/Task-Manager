import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import {
  ProjectDto,
  TaskItemDetailsDto,
  TaskItemStatus,
  UserDto
} from '../../core/models/api.models';
import { TasksService } from '../../core/services/tasks.service';
import { ProjectsService } from '../../core/services/projects.service';
import { UsersService } from '../../core/services/users.service';
import { TASK_STATUS_OPTIONS } from './task-status.constants';

export interface TaskDetailsDialogData {
  taskId: string;
}

@Component({
  selector: 'app-task-details-dialog',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="dialog-header">
      <h2 mat-dialog-title>
        {{ isEditMode() ? 'Редактирование задачи' : 'Информация о задаче' }}
      </h2>
    </div>

    <mat-dialog-content>
      @if (isLoading()) {
        <div class="loading-box">
          <mat-spinner diameter="36"></mat-spinner>
          <span>Загрузка карточки задачи...</span>
        </div>
      } @else {
        @if (task(); as currentTask) {
          @if (!isEditMode()) {
            <!-- Режим просмотра -->
            <div class="details-view">
              <h3 class="task-title">{{ currentTask.name }}</h3>

              <div class="meta-grid">
                <div class="meta-item">
                  <span class="meta-label">Проект</span>
                  <span class="meta-value">{{ currentTask.projectName }}</span>
                </div>
                <div class="meta-item">
                  <span class="meta-label">Дата создания</span>
                  <span class="meta-value">{{ currentTask.createdAt | date:'dd.MM.yyyy HH:mm' }}</span>
                </div>
              </div>

              <mat-divider class="divider"></mat-divider>

              <!-- Быстрая смена статуса и исполнителя -->
              <div class="quick-controls">
                <mat-form-field appearance="outline" class="control-field">
                  <mat-label>Статус</mat-label>
                  <mat-select [value]="currentTask.status"
                              [disabled]="isActionLoading()"
                              (selectionChange)="onQuickStatusChange($event.value)">
                    @for (option of statusOptions; track option.value) {
                      <mat-option [value]="option.value">{{ option.label }}</mat-option>
                    }
                  </mat-select>
                </mat-form-field>

                <mat-form-field appearance="outline" class="control-field">
                  <mat-label>Исполнитель</mat-label>
                  <mat-select [value]="currentTask.assigneeId"
                              [disabled]="isActionLoading()"
                              (selectionChange)="onQuickAssigneeChange($event.value)">
                    <mat-option [value]="null">Не назначен</mat-option>
                    @for (user of users(); track user.id) {
                      <mat-option [value]="user.id">{{ user.name }} ({{ user.email }})</mat-option>
                    }
                  </mat-select>
                </mat-form-field>
              </div>

              @if (currentTask.assigneeEmail) {
                <div class="assignee-email">
                  Email исполнителя: <strong>{{ currentTask.assigneeEmail }}</strong>
                </div>
              }

              <div class="description-section">
                <span class="meta-label">Описание</span>
                <p class="description-text">
                  {{ currentTask.description || 'Описание не указано.' }}
                </p>
              </div>
            </div>
          } @else {
            <!-- Режим редактирования -->
            <form [formGroup]="editForm" class="edit-form">
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Название задачи</mat-label>
                <input matInput formControlName="name" maxlength="200">
                @if (editForm.get('name')?.hasError('required') && editForm.get('name')?.touched) {
                  <mat-error>Название обязательно</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Проект</mat-label>
                <mat-select formControlName="projectId">
                  @for (project of projects(); track project.id) {
                    <mat-option [value]="project.id">{{ project.name }}</mat-option>
                  }
                </mat-select>
              </mat-form-field>

              <div class="quick-controls">
                <mat-form-field appearance="outline" class="control-field">
                  <mat-label>Статус</mat-label>
                  <mat-select formControlName="status">
                    @for (option of statusOptions; track option.value) {
                      <mat-option [value]="option.value">{{ option.label }}</mat-option>
                    }
                  </mat-select>
                </mat-form-field>

                <mat-form-field appearance="outline" class="control-field">
                  <mat-label>Исполнитель</mat-label>
                  <mat-select formControlName="assigneeId">
                    <mat-option [value]="null">Не назначен</mat-option>
                    @for (user of users(); track user.id) {
                      <mat-option [value]="user.id">{{ user.name }} ({{ user.email }})</mat-option>
                    }
                  </mat-select>
                </mat-form-field>
              </div>

              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Описание</mat-label>
                <textarea matInput formControlName="description" rows="5" maxlength="5000"></textarea>
              </mat-form-field>
          </form>
          }
        }
      }
    </mat-dialog-content>

    <mat-dialog-actions class="dialog-actions">
      @if (!isLoading() && task()) {
        @if (!isEditMode()) {
          <button mat-button color="warn" type="button" (click)="onDelete()" [disabled]="isActionLoading()">
            <mat-icon>delete</mat-icon>
            Удалить
          </button>
          <span class="spacer"></span>
          <button mat-stroked-button color="primary" type="button" (click)="enterEditMode()">
            <mat-icon>edit</mat-icon>
            Редактировать
          </button>
          <button mat-flat-button color="primary" type="button" (click)="onClose()">
            Закрыть
          </button>
        } @else {
          <span class="spacer"></span>
          <button mat-button type="button" (click)="isEditMode.set(false)" [disabled]="isActionLoading()">
            Отмена
          </button>
          <button mat-flat-button
                  color="primary"
                  type="button"
                  (click)="onSaveEdit()"
                  [disabled]="editForm.invalid || isActionLoading()">
            {{ isActionLoading() ? 'Сохранение...' : 'Сохранить' }}
          </button>
        }
      } @else {
        <span class="spacer"></span>
        <button mat-button type="button" (click)="onClose()">Закрыть</button>
      }
    </mat-dialog-actions>
  `,
  changeDetection: ChangeDetectionStrategy.Eager,
  styles: [`
    .details-view, .edit-form {
      min-width: 520px;
      padding-top: 4px;
    }
    .task-title {
      margin: 0 0 16px;
      font-size: 20px;
      font-weight: 500;
    }
    .meta-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      margin-bottom: 16px;
    }
    .meta-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .meta-label {
      font-size: 12px;
      color: #777;
      text-transform: uppercase;
      letter-spacing: 0.4px;
    }
    .meta-value {
      font-size: 15px;
      font-weight: 500;
    }
    .divider {
      margin-bottom: 16px;
    }
    .quick-controls {
      display: flex;
      gap: 12px;
    }
    .control-field {
      flex: 1;
    }
    .assignee-email {
      font-size: 13px;
      color: #555;
      margin: -8px 0 16px;
    }
    .description-section {
      display: flex;
      flex-direction: column;
      gap: 6px;
      margin-top: 8px;
    }
    .description-text {
      margin: 0;
      padding: 12px;
      background-color: #f8f9fa;
      border-radius: 4px;
      white-space: pre-wrap;
      line-height: 1.5;
      min-height: 60px;
    }
    .full-width {
      width: 100%;
      margin-bottom: 12px;
    }
    .loading-box {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 40px 16px;
      color: #666;
    }
    .dialog-actions {
      display: flex;
      align-items: center;
      width: 100%;
      box-sizing: border-box;
    }
    .spacer {
      flex: 1 1 auto;
    }
  `]
})
export class TaskDetailsDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private tasksService = inject(TasksService);
  private projectsService = inject(ProjectsService);
  private usersService = inject(UsersService);
  private snackBar = inject(MatSnackBar);
  private dialogRef = inject(MatDialogRef<TaskDetailsDialogComponent>);
  public data: TaskDetailsDialogData = inject(MAT_DIALOG_DATA);

  public readonly statusOptions = TASK_STATUS_OPTIONS;

  // Флаг для родительской таблицы: были ли изменения внутри карточки
  public hasChanges = false;

  public task = signal<TaskItemDetailsDto | null>(null);
  public projects = signal<ProjectDto[]>([]);
  public users = signal<UserDto[]>([]);
  public isLoading = signal<boolean>(true);
  public isEditMode = signal<boolean>(false);
  public isActionLoading = signal<boolean>(false);

  public editForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    projectId: [null, [Validators.required]],
    status: ['ToDo' as TaskItemStatus, [Validators.required]],
    assigneeId: [null as string | null],
    description: ['', [Validators.maxLength(5000)]]
  });

  ngOnInit(): void {
    // Загружаем детали задачи, а также проекты и пользователей для выбора
    forkJoin({
      task: this.tasksService.getTaskById(this.data.taskId),
      projects: this.projectsService.getProjects('', 1, 50),
      users: this.usersService.getUsers('', 1, 50)
    }).subscribe({
      next: ({ task, projects, users }) => {
        this.task.set(task);
        this.projects.set(projects.items);
        this.users.set(users.items);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  // Быстрая смена статуса через PATCH /api/v1/tasks/{id}/status
  public onQuickStatusChange(newStatus: TaskItemStatus): void {
    const current = this.task();
    if (!current || current.status === newStatus) return;

    this.isActionLoading.set(true);
    this.tasksService.changeStatus(current.id, newStatus).subscribe({
      next: (updated) => {
        this.task.set(updated);
        this.hasChanges = true;
        this.isActionLoading.set(false);
        this.snackBar.open('Статус задачи обновлен', 'ОК', { duration: 2500 });
      },
      error: () => {
        this.isActionLoading.set(false);
      }
    });
  }

  // Быстрое назначение или снятие исполнителя через PATCH /api/v1/tasks/{id}/assignee
  public onQuickAssigneeChange(assigneeId: string | null): void {
    const current = this.task();
    if (!current || current.assigneeId === assigneeId) return;

    this.isActionLoading.set(true);
    this.tasksService.assignTask(current.id, assigneeId).subscribe({
      next: (updated) => {
        this.task.set(updated);
        this.hasChanges = true;
        this.isActionLoading.set(false);
        this.snackBar.open(
          assigneeId ? 'Исполнитель назначен' : 'Исполнитель снят с задачи',
          'ОК',
          { duration: 2500 }
        );
      },
      error: () => {
        this.isActionLoading.set(false);
      }
    });
  }

  // Переход в режим полного редактирования с заполнением полей текущими значениями
  public enterEditMode(): void {
    const current = this.task();
    if (!current) return;

    this.editForm.patchValue({
      name: current.name,
      projectId: current.projectId,
      status: current.status,
      assigneeId: current.assigneeId,
      description: current.description ?? ''
    });
    this.isEditMode.set(true);
  }

  // Сохранение отредактированной задачи через PATCH /api/v1/tasks/{id}
  public onSaveEdit(): void {
    const current = this.task();
    if (!current || this.editForm.invalid) return;

    this.isActionLoading.set(true);
    const val = this.editForm.value;

    this.tasksService
      .updateTask(current.id, {
        name: val.name.trim(),
        projectId: val.projectId,
        status: val.status,
        assigneeId: val.assigneeId || null,
        description: val.description?.trim() || null
      })
      .subscribe({
        next: (updated) => {
          this.task.set(updated);
          this.hasChanges = true;
          this.isEditMode.set(false);
          this.isActionLoading.set(false);
          this.snackBar.open('Задача обновлена', 'ОК', { duration: 2500 });
        },
        error: () => {
          this.isActionLoading.set(false);
        }
      });
  }

  // Удаление задачи из карточки
  public onDelete(): void {
    const current = this.task();
    if (!current) return;

    if (!confirm(`Удалить задачу «${current.name}»?`)) return;

    this.isActionLoading.set(true);
    this.tasksService.deleteTask(current.id).subscribe({
      next: () => {
        this.hasChanges = true;
        this.snackBar.open('Задача удалена', 'ОК', { duration: 2500 });
        this.dialogRef.close(true);
      },
      error: () => {
        this.isActionLoading.set(false);
      }
    });
  }

  public onClose(): void {
    this.dialogRef.close(this.hasChanges);
  }
}
