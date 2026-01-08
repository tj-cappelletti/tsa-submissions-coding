import { AuthService } from '../../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProblemsService } from '../../services/problems.service';
import { Problem } from '../../models/problem.models';
import { Roles } from '../../models/auth.models';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-problems',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './problems.component.html',
  styleUrls: ['./problems.component.css']
})
export class ProblemsComponent implements OnInit {
  problems: Problem[] = [];
  loading = true;
  error: string | null = null;
  userRole: string | null = null;
  readonly Roles = Roles;

  constructor(private problemsService: ProblemsService, private authService: AuthService) { }

  ngOnInit() {
    this.userRole = this.authService.getUserRole();

    this.problemsService.getProblems().subscribe({
      next: (problems) => {
        this.problems = problems;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load problems.';
        this.loading = false;
      }
    });
  }
}
