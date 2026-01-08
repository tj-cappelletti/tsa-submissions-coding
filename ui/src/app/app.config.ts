import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideMarkdown } from 'ngx-markdown';

import { routes } from './app.routes';
import { MonacoEditorModule, NgxMonacoEditorConfig } from 'ngx-monaco-editor-v2';

export const monacoConfig: NgxMonacoEditorConfig = {
  baseUrl: window.location.origin + "/assets/monaco/min/vs"
};

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(MonacoEditorModule.forRoot(monacoConfig)),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideMarkdown()
  ]
};
