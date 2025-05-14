import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { PaginatedPlayersResponse, PlayerResponse } from '../interfaces/player.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class PlayerService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  private baseApiUrl = `${this.baseUrl}/api/player`;

  getPlayersPaginated(
    steamId: string | null = null,
    page: number = 1,
    pageSize: number = 10
  ): Observable<PaginatedPlayersResponse> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (steamId) {
      params = params.set('steamId', steamId);
    }

    return this.http.get<PaginatedPlayersResponse>(this.baseApiUrl, { params });
  }

  getIdByAuth(): Observable<number | null> {
    return this.http.get<number | null>(`${this.baseApiUrl}/me`);
  }

  getPlayer(id: number): Observable<PlayerResponse> {
    return this.http.get<PlayerResponse>(`${this.baseApiUrl}/${id}`);
  }
}