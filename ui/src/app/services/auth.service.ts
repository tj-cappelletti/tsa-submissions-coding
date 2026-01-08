import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { of, Observable } from 'rxjs';
import { Authentication, JwtPayload, LoginResponse } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly apiUrl: string = '/api/auth';
  readonly jwtKeyName = 'jwt';

  constructor(private http: HttpClient) { }

  getUserName(): string | null {
    const jwtPayload = this.getTokenPayload();

    if (jwtPayload === null) return null;

    return jwtPayload.unique_name;
  }

  getUserRole(): string | null {
    const jwtPayload = this.getTokenPayload();

    if (jwtPayload === null) return null;

    return jwtPayload.role;
  }

  getToken(): string | null {
    const token = localStorage.getItem(this.jwtKeyName);

    return token ? token : null;
  }

  private getTokenPayload(): JwtPayload | null {
    const token = this.getToken()

    if (!token) {
      console.log('No token found in local storage');
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1])) as JwtPayload;

      // Check if the payload is valid
      if (!payload) {
        console.log('Invalid token payload');
        return null;
      }

      if (!payload.exp || !payload.iat || !payload.role || !payload.unique_name) {
        console.log('Token payload is missing required fields');
        return null;
      }

      return payload;
    } catch (error) {
      console.error('Error parsing token:', error);
      return null;
    }
  }

  isAuthenticated(): boolean {
    const jwtPayload = this.getTokenPayload();

    if (jwtPayload === null) return false;

    try {
      // JWT exp is in seconds since epoch
      const now = Math.floor(Date.now() / 1000);

      if (jwtPayload.exp < now) {
        // Token expired
        return false;
      }

      return true;

    } catch {
      // If the token is malformed or invalid, treat it as not authenticated
      return false;
    }
  }

  login(username: string, password: string): Observable<boolean> {
    const authenticationModel: Authentication = {
      password: password,
      userName: username
    };
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, authenticationModel).pipe(
      map(res => {
        localStorage.setItem(this.jwtKeyName, res.token);
        return true;
      }),
      catchError(() => of(false))
    );
  }

  logout(): void {
    localStorage.removeItem(this.jwtKeyName);
  }
}
