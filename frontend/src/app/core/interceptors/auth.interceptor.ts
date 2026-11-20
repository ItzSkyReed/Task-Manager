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
      // Не пытаемся обновлять токен на самих эндпоинтах авторизации
      const isAuthUrl =
        req.url.includes('/auth/login') ||
        req.url.includes('/auth/register') ||
        req.url.includes('/auth/refresh') ||
        req.url.includes('/auth/logout');

      if (error.status === 401 && !isAuthUrl) {
        // Если другой параллельный запрос уже обновил токен, сразу повторяем запрос с ним
        const latestToken = authService.getToken();
        if (latestToken && latestToken !== token) {
          const retryReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${latestToken}`
            }
          });
          return next(retryReq);
        }

        // Иначе запускаем (или ждём уже запущенный) общий запрос обновления токена
        return authService.refresh().pipe(
          switchMap((newTokenResponse) => {
            const retryReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${newTokenResponse.accessToken}`
              }
            });
            return next(retryReq);
          }),
          catchError((refreshError) => {
            // Если refresh-токен недействителен — очищаем сессию и переводим на вход
            authService.logoutLocally();
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
