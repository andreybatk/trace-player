import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError } from 'rxjs';
import { throwError } from 'rxjs';

export const errorHandlerInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error instanceof HttpErrorResponse) {
        console.error('HTTP Error:', error);

        if (error.status === 404) {
          const errorMessage = error.error || 'Страница не найдена';
          router.navigate(['/not-found'], {
            state: { message: errorMessage }
          });
        }

        else if (error.status === 500) {
          router.navigate(['/not-found'], {
            state: { message: 'Внутренняя ошибка сервера' }
          });
        }

        else {
          router.navigate(['/not-found'], {
            state: { message: 'Произошла ошибка, попробуйте позже.' }
          });
        }
      }

      return throwError(() => error);
    })
  );
};