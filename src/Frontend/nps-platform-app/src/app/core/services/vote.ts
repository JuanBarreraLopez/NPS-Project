import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VoteService {
  private http = inject(HttpClient);
  private apiUrl = environment.npsApiUrl;

  submitVote(score: number, comment: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/submit`, { score, comment });
  }
}
