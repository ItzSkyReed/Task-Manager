import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateProjectRequest,
  PagedResponse,
  PatchProjectRequest,
  ProjectDto
} from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class ProjectsService {
  private http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/projects';

  // получение проектов с поиском
  public getProjects(search?: string, page = 1, pageSize = 10): Observable<PagedResponse<ProjectDto>> {
    let params = new HttpParams()
      .set('Page', page)
      .set('PageSize', pageSize);

    // Search только если строка поиска не пустая
    if (search && search.trim().length > 0) {
      params = params.set('Search', search.trim());
    }

    return this.http.get<PagedResponse<ProjectDto>>(this.baseUrl, { params });
  }

  // Получение одного проекта по ID
  public getProjectById(id: string): Observable<ProjectDto> {
    return this.http.get<ProjectDto>(`${this.baseUrl}/${id}`);
  }

  // Создание нового проекта
  public createProject(request: CreateProjectRequest): Observable<ProjectDto> {
    return this.http.post<ProjectDto>(this.baseUrl, request);
  }

  // Редактирование существующего проекта
  public updateProject(id: string, request: PatchProjectRequest): Observable<ProjectDto> {
    return this.http.patch<ProjectDto>(`${this.baseUrl}/${id}`, request);
  }

  // Удаление проекта
  public deleteProject(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
