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
    path: 'admin',
    loadComponent: () =>
      import('./features/admin/layout/admin-layout/admin-layout').then(
        (m) => m.AdminLayoutComponent
      ),
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/pages/admin-dashboard-page/admin-dashboard-page').then(
            (m) => m.AdminDashboardPageComponent
          ),
      },
      {
        path: 'register-voter',
        loadComponent: () =>
          import('./features/admin/pages/voter-registration/voter-registration').then(
            (m) => m.VoterRegistrationComponent
          ),
      },
    ],
  },

  {
    path: 'vote',
    loadComponent: () =>
      import('./features/vote/pages/vote-page/vote-page').then((m) => m.VotePageComponent),
    canActivate: [AuthGuard],
  },

  { path: '**', redirectTo: 'login' },
];
