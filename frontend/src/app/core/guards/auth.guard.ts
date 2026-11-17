import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Если пользователь авторизован (есть токен), пускаем дальше
  if (authService.getToken()) {
    return true;
  }

  // Иначе перенаправляем на страницу входа
  return router.parseUrl('/login');
};
