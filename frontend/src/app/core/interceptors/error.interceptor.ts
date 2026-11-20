import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';
import { HttpValidationProblemDetails } from '../models/api.models';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Произошла непредвиденная ошибка на сервере.';

      if (error.error) {
        const problem = error.error as HttpValidationProblemDetails;

        // Если есть детальное сообщение от бэка
        if (problem.detail) {
          errorMessage = problem.detail;
        } else if (problem.title) {
          errorMessage = problem.title;
        }

        // Если это ошибки валидации от fluent validation
        if (problem.errors) {
          const firstKey = Object.keys(problem.errors)[0];
          if (firstKey && problem.errors[firstKey].length > 0) {
            errorMessage = problem.errors[firstKey][0];
          }
        }
      } else if (error.message) {
        errorMessage = error.message;
      }

      // Не показываем ошибку 401 т.к она обрабатывается auth interceptoром
      if (error.status !== 401 || req.url.includes('/auth/login')) {
        snackBar.open(errorMessage, 'Закрыть', {
          duration: 5000,
          horizontalPosition: 'end',
          verticalPosition: 'top',
          panelClass: ['error-snackbar']
        });
      }

      return throwError(() => error);
    })
  );
};
