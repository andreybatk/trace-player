import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authTokenInterceptor } from './auth/auth.interceptor';
import { errorHandlerInterceptor } from './data/error-handler.interceptor';
import { apiKeyInterceptor } from './data/apiKey.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        apiKeyInterceptor,
        authTokenInterceptor,
        errorHandlerInterceptor
      ])
    )
  ]
};