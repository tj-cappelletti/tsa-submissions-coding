import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MarkdownModule } from 'ngx-markdown';
import { ProblemsService } from '../../services/problems.service';
import { Problem } from '../../models/problem.models';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-problem',
  imports: [CommonModule, MarkdownModule],
  templateUrl: './problem.component.html',
  styleUrl: './problem.component.css'
})
export class ProblemComponent {
  error: string | null = null;
  problem: Problem | null = null;

  constructor(private problemsService: ProblemsService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');

      if (!id) {
        this.problem = null;
        return;
      }

      this.problemsService.getProblemById(id).subscribe({
        next: (problem) => {
          this.problem = problem;
        },
        error: () => {
          this.error = 'Failed to load the problem.';
        }
      });
    });
  }
}
