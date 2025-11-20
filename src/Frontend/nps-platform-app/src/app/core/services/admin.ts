import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = environment.identityApiUrl.replace('/api/identity', '') + '/users';

  registerVoter(data: { username: string; password: string; role: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, data);
  }
}
