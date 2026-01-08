import { Component, Renderer2 } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from './services/auth.service';
import { ThemeService } from './services/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NgIf]
})
export class AppComponent {
  title = 'tsa-submissions-coding-ui';
  theme: 'light' | 'dark' = 'light';

  constructor(
    private renderer: Renderer2,
    private authService: AuthService,
    private themeService: ThemeService) {
    // Detect system/browser theme preference
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    this.theme = prefersDark ? 'dark' : 'light';
    this.setTheme(this.theme);

    // Listen for changes in system/browser theme
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
      const newTheme = e.matches ? 'dark' : 'light';
      this.theme = newTheme;
      this.setTheme(newTheme);
    });
  }

  toggleTheme() {
    this.theme = this.theme === 'light' ? 'dark' : 'light';
    this.setTheme(this.theme);
  }

  setTheme(theme: 'light' | 'dark') {
    this.themeService.setTheme(this.theme);
    this.renderer.setAttribute(document.body, 'data-theme', theme);
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }
}
