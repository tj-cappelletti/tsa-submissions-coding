import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { ProblemComponent } from './components/problems/problem.component';
import { ProblemsComponent } from './components/problems/problems.component';
import { ProblemFormComponent } from './components/problems/problem-form.component';
import { authGuard } from './guards/auth.guard';
import { judgeRoleGuard } from './guards/judge-role.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'problems', component: ProblemsComponent, canActivate: [authGuard] },
  { path: 'problems/new', component: ProblemFormComponent, canActivate: [judgeRoleGuard] },
  { path: 'problems/:id/edit', component: ProblemFormComponent, canActivate: [judgeRoleGuard] },
  { path: 'problems/:id', component: ProblemComponent, canActivate: [authGuard] },
];
