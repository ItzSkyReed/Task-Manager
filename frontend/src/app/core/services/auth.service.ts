import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import {
  AuthResponseDto,
  AccessTokenDto,
  LoginRequest,
  RegisterRequest,
  UserDto
} from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  // Сигнал для хранения текущего пользователя
  private currentUserSignal = signal<UserDto | null>(null);
  public currentUser = this.currentUserSignal.asReadonly();

  private accessTokenKey = 'access_token';

  // Получить текущий accessToken из localStorage
  public getToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  // Сохранить accessToken
  public setToken(token: string): void {
    localStorage.setItem(this.accessTokenKey, token);
  }

  // Вход в систему
  public login(request: LoginRequest): Observable<AuthResponseDto> {
    return this.http
      .post<AuthResponseDto>('/api/v1/auth/login', request, {
        withCredentials: true
      })
      .pipe(
        tap((response) => {
          this.setToken(response.accessToken);
          this.currentUserSignal.set(response.user);
        })
      );
  }

  // Регистрация
  public register(request: RegisterRequest): Observable<AuthResponseDto> {
    return this.http
      .post<AuthResponseDto>('/api/v1/auth/register', request, {
        withCredentials: true
      })
      .pipe(
        tap((response) => {
          this.setToken(response.accessToken);
          this.currentUserSignal.set(response.user);
        })
      );
  }

  // Обновление токена (вызывается при 401)
  public refresh(): Observable<AccessTokenDto> {
    return this.http
      .post<AccessTokenDto>(
        '/api/v1/auth/refresh',
        {},
        { withCredentials: true }
      )
      .pipe(
        tap((response) => {
          this.setToken(response.accessToken);
        })
      );
  }

  // Получить данные текущего пользователя
  public getMe(): Observable<UserDto> {
    return this.http.get<UserDto>('/api/v1/users/me').pipe(
      tap((user) => {
        this.currentUserSignal.set(user);
      })
    );
  }

  // Выход из системы
  public logout(): Observable<void> {
    return this.http
      .post<void>(
        '/api/v1/auth/logout',
        {},
        { withCredentials: true }
      )
      .pipe(
        tap(() => {
          this.logoutLocally();
        })
      );
  }

  // Локальная очистка сессии и редирект на логин
  public logoutLocally(): void {
    localStorage.removeItem(this.accessTokenKey);
    this.currentUserSignal.set(null);
    this.router.navigate(['/login']);
  }
}
