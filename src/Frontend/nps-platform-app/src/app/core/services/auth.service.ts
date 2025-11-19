import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

interface LoginRequest { username: string; password: string; }
interface RegisterRequest { email: string; password: string; }
interface AuthResult { token: string; email: string; }

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly identityUrl = environment.identityApiUrl;
  private readonly TOKEN_KEY = 'jwt_token';

  isAuthenticated = signal<boolean>(false);

  constructor(private http: HttpClient, private router: Router) {
    this.checkAuthStatus();
  }

  private checkAuthStatus(): void {
    const token = this.getToken();
    this.isAuthenticated.set(!!token);
  }

  login(credentials: LoginRequest): Observable<AuthResult> {
    const url = `${this.identityUrl}/login`;
    return this.http.post<AuthResult>(url, credentials).pipe(
      tap(result => {
        this.setToken(result.token);
        this.isAuthenticated.set(true);
        this.router.navigate(['/dashboard']);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }
}
