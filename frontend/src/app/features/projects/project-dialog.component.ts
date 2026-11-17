import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ProjectDto } from '../../core/models/api.models';
import { ProjectsService } from '../../core/services/projects.service';

@Component({
  selector: 'app-project-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  // Заголовок меняется в зависимости от того, передан ли проект для редактирования
  template: `

    <h2 mat-dialog-title>{{ isEditMode ? 'Редактирование проекта' : 'Создание проекта' }}</h2>

    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Название проекта</mat-label>
          <input matInput formControlName="name" placeholder="Например: Разработка CRM" maxlength="200">
          @if (form.get('name')?.hasError('required') && form.get('name')?.touched) {
            <mat-error>Название проекта обязательно</mat-error>
          }
          @if (form.get('name')?.hasError('maxlength')) {
            <mat-error>Максимум 200 символов</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Описание</mat-label>
          <textarea matInput
                    formControlName="description"
                    rows="4"
                    placeholder="Краткое описание целей проекта (необязательно)"
                    maxlength="5000"></textarea>
          @if (form.get('description')?.hasError('maxlength')) {
            <mat-error>Максимум 5000 символов</mat-error>
          }
        </mat-form-field>
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="onCancel()" [disabled]="isSaving">
        Отмена
      </button>
      <button mat-flat-button
              color="primary"
              type="button"
              (click)="onSave()"
              [disabled]="form.invalid || isSaving">
        {{ isSaving ? 'Сохранение...' : (isEditMode ? 'Сохранить' : 'Создать') }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-form {
      display: flex;
      flex-direction: column;
      min-width: 400px;
      padding-top: 8px;
    }
    .full-width {
      width: 100%;
      margin-bottom: 12px;
    }
  `]
})
export class ProjectDialogComponent {
  private fb = inject(FormBuilder);
  private projectsService = inject(ProjectsService);
  private dialogRef = inject(MatDialogRef<ProjectDialogComponent>);

  // Получаем данные, переданные при открытии диалога (ProjectDto при редактировании или null при создании)
  public data: ProjectDto | null = inject(MAT_DIALOG_DATA, { optional: true });

  public isEditMode = Boolean(this.data);
  public isSaving = false;

  // Инициализируем форму значениями из data, если это режим редактирования
  public form: FormGroup = this.fb.group({
    name: [this.data?.name ?? '', [Validators.required, Validators.maxLength(200)]],
    description: [this.data?.description ?? '', [Validators.maxLength(5000)]]
  });

  // Закрытие окна без сохранения
  public onCancel(): void {
    this.dialogRef.close(false);
  }

  // Отправка данных
  public onSave(): void {
    if (this.form.invalid) return;

    this.isSaving = true;
    const payload = {
      name: this.form.value.name.trim(),
      description: this.form.value.description?.trim() || null
    };

    const request$ = this.isEditMode && this.data
      ? this.projectsService.updateProject(this.data.id, payload)
      : this.projectsService.createProject(payload);

    request$.subscribe({
      next: () => {
        // Передаем true родительскому компоненту как сигнал, что таблицу нужно обновить
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSaving = false;
      }
    });
  }
}
