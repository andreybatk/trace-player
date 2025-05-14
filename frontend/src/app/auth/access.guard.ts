import { inject } from "@angular/core"
import { AuthService } from "./auth.service"
import { Router } from "@angular/router"

export const canActivateAuth = () => {
  const isLoggedIn = inject(AuthService).isAuth

  if(isLoggedIn) {
      return true
  }

  return inject(Router).navigate(['/access-denied'])
}

export const canActivateGuest = () => {
  const isLoggedIn = inject(AuthService).isAuth;

  if (!isLoggedIn) {
    return true;
  }

  return inject(Router).navigate(['/']);
};

export const canActivateRole = (requiredRoles: string[]) => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isAuth) {
      return router.navigate(['/access-denied']);
    }

    const userRoles = authService.userRoles;

    const hasRequiredRole = requiredRoles.some(role => userRoles.includes(role));

    if (hasRequiredRole) {
      return true;
    }

    return router.navigate(['/access-denied']);
  };
};