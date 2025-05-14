import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs';
import { throwError } from 'rxjs';
import { ErrorService } from './services/error.service';

export const errorHandlerInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(ErrorService);
  
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.log("Error request", error);
      
      if (error instanceof HttpErrorResponse) {
        let message = 'Произошла ошибка';

        if (error.status === 400) {
          message = error.error;
        } 
        else if (error.status === 404) {
        message = error.error || 'Страница не найдена';
        }    
        else if (error.status === 500) {
          message = 'Внутренняя ошибка сервера';
        }

        errorService.show(message);
      }

      return throwError(() => error);
    })
  );
};