import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/pages/login-page/login-page').then((m) => m.LoginPageComponent),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/pages/admin-dashboard-page/admin-dashboard-page').then(
        (m) => m.AdminDashboardPageComponent
      ),
    canActivate: [AuthGuard],
  },
  { path: '**', redirectTo: 'login' },
];
