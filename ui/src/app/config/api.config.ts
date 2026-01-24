import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ApiConfig {
  apiBaseUrl: string = '/api';

  constructor() {
    // Try to get from window object (set by index.html or startup)
    const windowConfig = (window as any).__API_URL__;
    if (windowConfig) {
      this.apiBaseUrl = windowConfig;
    }
  }
}
