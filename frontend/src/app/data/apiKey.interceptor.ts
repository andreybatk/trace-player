import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../environments/environment';

export const apiKeyInterceptor: HttpInterceptorFn = (req, next) => {
  const apiKey = environment.apiKey;

  if (!apiKey) return next(req);

  const authReq = req.clone({
    setHeaders: {
      'X-API-Key': apiKey
    }
  });

  return next(authReq);
};