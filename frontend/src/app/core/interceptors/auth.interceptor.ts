import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  // Клонируем запрос и добавляем заголовок авторизации, если токен есть
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Игнорируем ошибку 401 для самих эндпоинтов логина/рефреша, чтобы избежать бесконечного цикла
      const isAuthUrl = req.url.includes('/auth/login') || req.url.includes('/auth/refresh');

      if (error.status === 401 && !isAuthUrl) {
        // Пробуем обновить токен через рефреш куки
        return authService.refresh().pipe(
          switchMap((newTokenResponse) => {
            // Повторяем исходный запрос с новым токеном
            const retryReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${newTokenResponse.accessToken}`
              }
            });
            return next(retryReq);
          }),
          catchError((refreshError) => {
            // Если рефреш не удался то разлогиниваемся
            authService.logoutLocally();
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
