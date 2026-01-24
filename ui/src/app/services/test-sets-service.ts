import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { of, Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { TestSetModel } from '../models/test-set.models';

@Injectable({ providedIn: 'root' })
export class TestSetsService {
    readonly apiUrl: string = 'http://api.tsa.localdev.me:5000/api/testsets';

    constructor(private http: HttpClient, private authService: AuthService) { }

    private createHeaders(): HttpHeaders {
        const token = this.authService.getToken();
        return token ? new HttpHeaders({ 'Authorization': `Bearer ${token}` }) : new HttpHeaders();
    }

    createTestSet(testSet: TestSetModel) {
        const token = this.authService.getToken();
        const headers = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
        return this.http.post<TestSetModel>(`${this.apiUrl}`, testSet, headers);
    }

    getTestSets(): Observable<TestSetModel[]> {
        const headers = this.createHeaders();
        return this.http.get<TestSetModel[]>(this.apiUrl, { headers }).pipe(
            map((response: TestSetModel[]) => response),
            catchError((error) => {
                console.error('Error fetching test sets:', error);
                return of([]);
            })
        );
    }

    getTestSetById(id: string): Observable<TestSetModel | null> {
        const headers = this.createHeaders();
        return this.http.get<TestSetModel>(`${this.apiUrl}/${id}`, { headers }).pipe(
            map((response: TestSetModel) => response),
            catchError((error) => {
                console.error('Error fetching TestSet:', error);
                return of(null);
            })
        );
    }

    updateTestSet(testSet: TestSetModel) {
        const token = this.authService.getToken();
        const headers = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
        return this.http.put<TestSetModel>(`${this.apiUrl}/${testSet.id}`, testSet, headers);
    }
}