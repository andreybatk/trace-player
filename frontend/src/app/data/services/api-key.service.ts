import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiKeyService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  private baseApiUrl = `${this.baseUrl}/api/api-key`;

  addApiKey(serverIp: string): Observable<string> {
    const params = new HttpParams().set('serverIp', serverIp);
    return this.http.post(this.baseApiUrl, null, { params, responseType: 'text' });
  }

  getAllServerIps(): Observable<string[]> {
    return this.http.get<string[]>(this.baseApiUrl);
  }

  deleteApiKey(serverIp: string): Observable<string[]> {
    return this.http.delete<string[]>(`${this.baseApiUrl}/${serverIp}`);
  }
}
