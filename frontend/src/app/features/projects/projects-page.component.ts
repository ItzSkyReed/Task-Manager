import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
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

import { ProjectDto } from '../../core/models/api.models';
import { ProjectsService } from '../../core/services/projects.service';
import { ProjectDialogComponent } from './project-dialog.component';

@Component({
  selector: 'app-projects-page',
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
      <div class="header-actions">
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Поиск по названию проекта</mat-label>
          <input matInput [formControl]="searchControl" placeholder="Введите название...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        <button mat-flat-button color="primary" (click)="openProjectDialog()">
          <mat-icon>add</mat-icon>
          Создать проект
        </button>
      </div>

      <!-- Полоска загрузки над таблицей -->
      @if (isLoading()) {
        <mat-progress-bar mode="indeterminate"></mat-progress-bar>
      }

      <!-- Таблица проектов -->
      <div class="table-wrapper mat-elevation-z2">
        <table mat-table [dataSource]="projects()">
          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef>Название</th>
            <td mat-cell *matCellDef="let project">
              <strong>{{ project.name }}</strong>
            </td>
          </ng-container>

          <ng-container matColumnDef="description">
            <th mat-header-cell *matHeaderCellDef>Описание</th>
            <td mat-cell *matCellDef="let project" class="description-cell">
              {{ project.description || '—' }}
            </td>
          </ng-container>

          <ng-container matColumnDef="tasksCount">
            <th mat-header-cell *matHeaderCellDef>Задач</th>
            <td mat-cell *matCellDef="let project">{{ project.tasksCount }}</td>
          </ng-container>

          <ng-container matColumnDef="createdAt">
            <th mat-header-cell *matHeaderCellDef>Создан</th>
            <td mat-cell *matCellDef="let project">
              {{ project.createdAt | date:'dd.MM.yyyy HH:mm' }}
            </td>
          </ng-container>

          <!--действия -->
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="actions-header">Действия</th>
            <td mat-cell *matCellDef="let project" class="actions-cell">
              <button mat-icon-button
                      color="primary"
                      title="Редактировать"
                      (click)="openProjectDialog(project)">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button
                      color="warn"
                      title="Удалить"
                      (click)="deleteProject(project)">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>

          <!-- Строка при пустом списке -->
          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell empty-cell" [attr.colspan]="displayedColumns.length">
              {{ isLoading() ? 'Загрузка...' : 'Проекты не найдены' }}
            </td>
          </tr>
        </table>

        <!-- пагинация -->
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
    .description-cell {
      max-width: 400px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      color: #555;
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
export class ProjectsPageComponent implements OnInit {
  private projectsService = inject(ProjectsService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private destroyRef = inject(DestroyRef);

  // Сигналы состояния таблицы и пагинации
  public projects = signal<ProjectDto[]>([]);
  public totalCount = signal<number>(0);
  public pageIndex = signal<number>(0);
  public pageSize = signal<number>(10);
  public isLoading = signal<boolean>(false);

  public searchControl = new FormControl<string>('', { nonNullable: true });
  public displayedColumns: string[] = ['name', 'description', 'tasksCount', 'createdAt', 'actions'];

  ngOnInit(): void {
    // подписываемся на ввод в поле
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300), // задержка 300 сек чтобы не слать постоянно запросы
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => {
        this.pageIndex.set(0); // При новом поиске сбрасываем на первую страницу
        this.loadProjects();
      });

    this.loadProjects();
  }

  // Загрузка списка проектов с бэкенда
  public loadProjects(): void {
    this.isLoading.set(true);

    // +1 к pageIndex, так как на бэкенде нумерация страниц с 1
    this.projectsService
      .getProjects(this.searchControl.value, this.pageIndex() + 1, this.pageSize())
      .subscribe({
        next: (response) => {
          this.projects.set(response.items);
          this.totalCount.set(response.totalCount);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
  }

  // Обработка переключения страницы или размера страницы в MatPaginator
  public onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadProjects();
  }

  // Открытие модального окна для создания (если  не передан проект) или редактирования (если передан)
  public openProjectDialog(project?: ProjectDto): void {
    const dialogRef = this.dialog.open(ProjectDialogComponent, {
      data: project ?? null,
      width: '480px'
    });

    dialogRef.afterClosed().subscribe((isSaved: boolean) => {
      if (isSaved) {
        this.snackBar.open(
          project ? 'Проект успешно обновлен' : 'Проект успешно создан',
          'ОК',
          { duration: 3000 }
        );
        this.loadProjects();
      }
    });
  }

  // Удаление проекта с подтверждением
  public deleteProject(project: ProjectDto): void {
    const confirmed = confirm(`Удалить проект "${project.name}" и все связанные с ним задачи?`);
    if (!confirmed) return;

    this.projectsService.deleteProject(project.id).subscribe({
      next: () => {
        this.snackBar.open('Проект удален', 'ОК', { duration: 3000 });
        this.loadProjects();
      }
    });
  }
}
