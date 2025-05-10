import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { JwtPayload, TokenResponse } from './auth.interface';
import { tap, throwError, catchError } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})

export class AuthService{
  http = inject(HttpClient)
  cookieService = inject(CookieService)
  router = inject(Router)

  private baseUrl = environment.apiUrl
  baseApiUrl = `${this.baseUrl}/api/auth/`

  token: string | null = null;
  refreshToken: string | null = null;

  constructor() {
    this.token = this.cookieService.get('token');
    this.refreshToken = this.cookieService.get('refreshToken');
  }
  
  get isAuth() {
    if(!this.token)
    {
      this.token = this.cookieService.get('token')
      this.refreshToken = this.cookieService.get('refreshToken')
    }

    return !!this.token
  }
  
  get username(): string | null {
    return this.cookieService.get('username') || null;
  }

  loginToSteam() {
    window.location.href = `${environment.apiUrl}/api/auth/steam-login?returnUrl=${environment.Url}/steam-callback`;
  }

  refreshAuthToken() {
    return this.http.post<TokenResponse>(
      `${this.baseApiUrl}refresh`,
      {
        refreshToken: this.refreshToken
      })
      .pipe(
          tap(val => this.saveTokens(val)),
          catchError(error => {
            this.logout()
            return throwError(() => new Error(error))
          })
      )
  }

  logout() {
    this.cookieService.deleteAll()
    this.token = null
    this.refreshToken = null;
    this.router.navigate(['/login'])
  }

  saveTokens(res: TokenResponse, username: string | null = null) {
    this.token = res.accessToken
    this.refreshToken = res.refreshToken
    
    this.cookieService.set('token', this.token)
    this.cookieService.set('refreshToken', this.refreshToken)

    if(username)
    {
      this.cookieService.set('username', username);
    }
  }

  get userRoles(): string[] {
    if (!this.token) {
      this.token = this.cookieService.get('token');
    }
  
    if (!this.token) return [];
  
    try {
      const decoded = jwtDecode<JwtPayload>(this.token);
      const roles = decoded.role;
      if (!roles) return [];
  
      return Array.isArray(roles) ? roles : [roles];
    } catch (e) {
      return [];
    }
  }
  
  get isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  hasRole(role: string): boolean {
    return this.userRoles.includes(role);
  }
}
