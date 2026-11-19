import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateUserRequest,
  PagedResponse,
  UserDto
} from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/users';

  public getUsers(search?: string, page = 1, pageSize = 10): Observable<PagedResponse<UserDto>> {
    let params = new HttpParams()
      .set('Page', page)
      .set('PageSize', pageSize);

    if (search && search.trim().length > 0) {
      params = params.set('Search', search.trim());
    }

    return this.http.get<PagedResponse<UserDto>>(this.baseUrl, { params });
  }

  public createUser(request: CreateUserRequest): Observable<UserDto> {
    return this.http.post<UserDto>(this.baseUrl, request);
  }

  public deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
