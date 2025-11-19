import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiUrl = environment.dashboardApiUrl;
  private http = inject(HttpClient);

  getSummary(): Observable<any> {
    const url = `${this.apiUrl}/summary`;
    return this.http.get(url);
  }

  getRecentVotes(): Observable<any> {
    const url = `${this.apiUrl}/recent-votes`;
    return this.http.get(url);
  }
}
