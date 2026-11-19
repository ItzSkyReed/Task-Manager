import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AssignTaskItemRequest,
  ChangeTaskItemStatusRequest,
  CreateTaskItemRequest,
  PagedResponse,
  PatchTaskItemRequest,
  TaskItemDetailsDto,
  TaskItemStatus,
  TaskItemSummaryDto
} from '../models/api.models';

// Параметры фильтрации и пагинации для GET /api/v1/tasks
export interface GetTasksParams {
  search?: string;
  projectId?: string | null;
  assigneeId?: string | null;
  status?: TaskItemStatus | null;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root'
})
export class TasksService {
  private http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/tasks';

  // Поиск
  public getTasks(query: GetTasksParams = {}): Observable<PagedResponse<TaskItemSummaryDto>> {
    let params = new HttpParams()
      .set('Page', query.page ?? 1)
      .set('PageSize', query.pageSize ?? 10);

    if (query.search && query.search.trim().length > 0) {
      params = params.set('Search', query.search.trim());
    }
    if (query.projectId) {
      params = params.set('ProjectId', query.projectId);
    }
    if (query.assigneeId) {
      params = params.set('AssigneeId', query.assigneeId);
    }
    if (query.status) {
      params = params.set('Status', query.status);
    }

    return this.http.get<PagedResponse<TaskItemSummaryDto>>(this.baseUrl, { params });
  }

  // Получение подробной карточки задачи
  public getTaskById(id: string): Observable<TaskItemDetailsDto> {
    return this.http.get<TaskItemDetailsDto>(`${this.baseUrl}/${id}`);
  }

  // Создание новой задачи
  public createTask(request: CreateTaskItemRequest): Observable<TaskItemDetailsDto> {
    return this.http.post<TaskItemDetailsDto>(this.baseUrl, request);
  }

  // Полное редактирование задачи
  public updateTask(id: string, request: PatchTaskItemRequest): Observable<TaskItemDetailsDto> {
    return this.http.patch<TaskItemDetailsDto>(`${this.baseUrl}/${id}`, request);
  }

  // Быстрая смена статуса задачи
  public changeStatus(id: string, status: TaskItemStatus): Observable<TaskItemDetailsDto> {
    const body: ChangeTaskItemStatusRequest = { status };
    return this.http.patch<TaskItemDetailsDto>(`${this.baseUrl}/${id}/status`, body);
  }

  // Назначение или снятие исполнителя с задачи
  public assignTask(id: string, assigneeId: string | null): Observable<TaskItemDetailsDto> {
    const body: AssignTaskItemRequest = { assigneeId };
    return this.http.patch<TaskItemDetailsDto>(`${this.baseUrl}/${id}/assignee`, body);
  }

  // Удаление задачи
  public deleteTask(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
