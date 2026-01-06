import { Component, Input, OnInit, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MarkdownModule } from 'ngx-markdown';
import { ProblemsService } from '../../services/problems.service';
import { ThemeService } from '../../services/theme.service';
import { Problem } from '../../models/problem.models';
import { ActivatedRoute } from '@angular/router';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';

@Component({
  selector: 'app-problem-form',
  standalone: true,
  imports: [CommonModule, FormsModule, MarkdownModule, MonacoEditorModule],
  templateUrl: './problem-form.component.html',
  styleUrls: ['./problem-form.component.css'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ProblemFormComponent implements OnInit {
  description = '';
  error: string | null = null;
  success: string | null = null;
  testSetsJson: string = '';
  title = '';

  readonly id: string | null;

  monacoTheme = 'vs-light';

  constructor(
    private problemsService: ProblemsService,
    private route: ActivatedRoute,
    private themeService: ThemeService
  ) {
    this.id = this.route.snapshot.paramMap.get('id');
    this.themeService.theme$.subscribe(theme => {
      this.monacoTheme = theme === 'dark' ? 'vs-dark' : 'vs-light';
    });
  }

  ngOnInit() {
    if (this.id) {
      this.problemsService.getProblemById(this.id).subscribe({
        next: (problem) => {
          if (problem) {
            this.title = problem.title;
            this.description = problem.description;
          }
        },
        error: () => {
          this.error = 'Failed to load problem.';
        }
      });
    }
  }

  onSubmit() {
    this.error = null;
    this.success = null;
    if (!this.title.trim() || !this.description.trim()) {
      this.error = 'Title and description are required.';
      return;
    }

    const problemData: Problem = { title: this.title, description: this.description };
    if (this.id) {
      // Edit
      this.problemsService.updateProblem(problemData).subscribe({
        next: () => this.success = 'Problem updated successfully!',
        error: () => this.error = 'Failed to update problem.'
      });
    } else {
      // Add
      this.problemsService.createProblem(problemData).subscribe({
        next: () => {
          this.success = 'Problem created successfully!';
          this.title = '';
          this.description = '';
        },
        error: () => this.error = 'Failed to create problem.'
      });
    }
  }

  setTheme(isDarkMode: boolean) {
    this.monacoTheme = isDarkMode ? 'vs-dark' : 'vs-light';
  }
}
