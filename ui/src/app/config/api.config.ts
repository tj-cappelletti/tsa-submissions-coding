import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ApiConfig {
  apiBaseUrl: string = '/api';

  constructor() {
    // Try to get from window object (set by index.html or startup)
    const windowConfig = (window as any).__API_URL__;
    if (windowConfig) {
      this.apiBaseUrl = this.normalize(windowConfig);
    }
  }

  getApiEndpoint(resource: string): string {
    const normalizedResource = resource.replace(/^\/+/, '');
    const baseUrl = this.normalize(this.apiBaseUrl);

    if (baseUrl.endsWith('/api')) {
      return `${baseUrl}/${normalizedResource}`;
    }

    return `${baseUrl}/api/${normalizedResource}`;
  }

  private normalize(url: string): string {
    return url.replace(/\/+$/, '');
  }
}
