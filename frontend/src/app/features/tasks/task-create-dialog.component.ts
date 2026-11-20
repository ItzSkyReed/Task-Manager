import { Component, OnInit, inject, signal } from '@angular/core';

import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { ProjectDto, TaskItemStatus, UserDto } from '../../core/models/api.models';
import { TasksService } from '../../core/services/tasks.service';
import { ProjectsService } from '../../core/services/projects.service';
import { UsersService } from '../../core/services/users.service';
import { TASK_STATUS_OPTIONS } from './task-status.constants';

@Component({
  selector: 'app-task-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule
],
  template: `
    <h2 mat-dialog-title>Создание задачи</h2>

    <mat-dialog-content>
      @if (isLoadingData()) {
        <div class="loading-box">
          <mat-spinner diameter="36"></mat-spinner>
          <span>Загрузка проектов и пользователей...</span>
        </div>
      } @else {
        <form [formGroup]="form" class="dialog-form">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Название задачи</mat-label>
            <input matInput formControlName="name" placeholder="Например: Настроить авторизацию" maxlength="200">
            @if (form.get('name')?.hasError('required') && form.get('name')?.touched) {
              <mat-error>Название задачи обязательно</mat-error>
            }
          </mat-form-field>

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Проект</mat-label>
            <mat-select formControlName="projectId">
              @for (project of projects(); track project.id) {
                <mat-option [value]="project.id">{{ project.name }}</mat-option>
              }
            </mat-select>
            @if (form.get('projectId')?.hasError('required') && form.get('projectId')?.touched) {
              <mat-error>Необходимо выбрать проект</mat-error>
            }
            @if (projects().length === 0) {
              <mat-hint>Сначала создайте проект на вкладке "Проекты"</mat-hint>
            }
          </mat-form-field>

          <div class="row-fields">
            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Статус</mat-label>
              <mat-select formControlName="status">
                @for (option of statusOptions; track option.value) {
                  <mat-option [value]="option.value">{{ option.label }}</mat-option>
                }
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline" class="half-width">
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
            <textarea matInput
                      formControlName="description"
                      rows="4"
                      placeholder="Подробное описание задачи (необязательно)"
                      maxlength="5000"></textarea>
          </mat-form-field>
        </form>
      }
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="onCancel()" [disabled]="isSaving()">
        Отмена
      </button>
      <button mat-flat-button
              color="primary"
              type="button"
              (click)="onSave()"
              [disabled]="form.invalid || isLoadingData() || isSaving()">
        {{ isSaving() ? 'Создание...' : 'Создать' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-form {
      display: flex;
      flex-direction: column;
      min-width: 480px;
      padding-top: 8px;
    }
    .full-width {
      width: 100%;
      margin-bottom: 12px;
    }
    .row-fields {
      display: flex;
      gap: 12px;
    }
    .half-width {
      flex: 1;
      margin-bottom: 12px;
    }
    .loading-box {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 32px 16px;
      color: #666;
    }
  `]
})
export class TaskCreateDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private tasksService = inject(TasksService);
  private projectsService = inject(ProjectsService);
  private usersService = inject(UsersService);
  private dialogRef = inject(MatDialogRef<TaskCreateDialogComponent>);

  public readonly statusOptions = TASK_STATUS_OPTIONS;

  // Сигналы для списков выпадающих меню и состояний загрузки
  public projects = signal<ProjectDto[]>([]);
  public users = signal<UserDto[]>([]);
  public isLoadingData = signal<boolean>(true);
  public isSaving = signal<boolean>(false);

  public form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    projectId: [null, [Validators.required]],
    status: ['ToDo' as TaskItemStatus, [Validators.required]],
    assigneeId: [null as string | null],
    description: ['', [Validators.maxLength(5000)]]
  });

  ngOnInit(): void {
    // Параллельно загружаем проекты и пользователей
    forkJoin({
      projects: this.projectsService.getProjects('', 1, 50),
      users: this.usersService.getUsers('', 1, 50)
    }).subscribe({
      next: ({ projects, users }) => {
        this.projects.set(projects.items);
        this.users.set(users.items);
        this.isLoadingData.set(false);
      },
      error: () => {
        this.isLoadingData.set(false);
      }
    });
  }

  public onCancel(): void {
    this.dialogRef.close(false);
  }

  public onSave(): void {
    if (this.form.invalid) return;

    this.isSaving.set(true);
    const val = this.form.value;

    this.tasksService
      .createTask({
        name: val.name.trim(),
        projectId: val.projectId,
        status: val.status,
        assigneeId: val.assigneeId || null,
        description: val.description?.trim() || null
      })
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
  }
}
