import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { of, Observable } from 'rxjs';
import { Problem } from '../models/problem.models';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class ProblemsService {
    readonly apiUrl: string = '/api/problems';

    constructor(private http: HttpClient, private authService: AuthService) { }

    private createHeaders(): HttpHeaders {
        const token = this.authService.getToken();
        return token ? new HttpHeaders({ 'Authorization': `Bearer ${token}` }) : new HttpHeaders();
    }

    createProblem(problem: Problem) {
        const token = this.authService.getToken();
        const headers = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
        return this.http.post<Problem>(this.apiUrl, problem, headers);
    }

    getProblems(): Observable<Problem[]> {
        const headers = this.createHeaders();
        return this.http.get<Problem[]>(this.apiUrl, { headers }).pipe(
            map((response: Problem[]) => response),
            catchError((error) => {
                console.error('Error fetching problems:', error);
                return of([]);
            })
        );
    }

    getProblemById(id: string): Observable<Problem | null> {
        const headers = this.createHeaders();
        return this.http.get<Problem>(`${this.apiUrl}/${id}`, { headers }).pipe(
            map((response: Problem) => response),
            catchError((error) => {
                console.error('Error fetching problem:', error);
                return of(null);
            })
        );
    }

    updateProblem(problem: Problem) {
        const token = this.authService.getToken();
        const headers = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
        return this.http.put<Problem>(`${this.apiUrl}/${problem.id}`, problem, headers);
    }
}
