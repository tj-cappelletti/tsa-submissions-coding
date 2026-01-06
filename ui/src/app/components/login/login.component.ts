import { AuthService } from '../../services/auth.service';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username = '';
  password = '';
  error: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.error = null;
    this.authService.login(this.username, this.password).subscribe({
      next: (loginSuccessful) => {
        if(loginSuccessful) {
          this.router.navigate(['/dashboard']);
        }
        else {
          this.error = 'Login failed. Please check your credentials.';
        }
      },
      error: () => {
        this.error = 'An error occurred during login. Please try again.';
      }
    });
  }
}
