import { Routes } from "@angular/router";
import { RouteGuard } from "../infrastructure/services/auth-guard.service";
import { LoginComponent } from "./pages/login/login.component";
import { ForgotPasswordComponent } from "./pages/forgot-password/forgot-password.component";
import { NotFoundComponent } from "./pages/not-found/not-found.component";
import { ResetPasswordComponent } from "./pages/reset-password/reset-password.component";
import { LoginHistoryComponent } from "./pages/login-history/login-history.component";

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/layout.component').then(e => e.LayoutComponent),
    canActivate: RouteGuard,
    children: [
      {
        path: 'login-history',
        title: 'گزارش ورود و خروج کاربر',
        component: LoginHistoryComponent
      },
      {
        path: '', title: 'میزکار',
        loadComponent: () =>
          import('./pages/dashboard/dashboard.component')
            .then(e => e.DashboardComponent)
      },
      {
        path: 'cms', title:
          'مدیریت محتوا', loadChildren: () =>
          import('../modules/cms/cms.routes')
            .then(e => e.routes)
      },
      {
        path: 'market-intelligence',
        title: 'تحلیل بازار',
        loadChildren: () =>
          import('../modules/MarketIntelligence/market-intelligence.routes')
            .then(m => m.routes)
      },
      {
        path: 'membership',
        title: 'مدیریت کاربران', loadChildren: () =>
          import('../modules/membership/membership.routes')
            .then(e => e.routes)
      },
    ]
  },
  {
    path: 'login',
    title: 'ورود', loadComponent: () =>
      LoginComponent, data: { allowAnonymous: true }
  },
  {
    path: 'forgot-password',
    title: 'فراموشی رمز عبور', loadComponent: () =>
      ForgotPasswordComponent, data: { allowAnonymous: true },
  },
  {
    path: 'reset-password',
    title: 'بازنشانی رمز عبور', loadComponent: () =>
      ResetPasswordComponent, data: { allowAnonymous: true }
  },
  {
    path: 'not-found',
    title: 'محتوای ناموجود', loadComponent: () =>
      NotFoundComponent, data: { allowAnonymous: true }
  },
  { path: '**', redirectTo: '/not-found', data: { allowAnonymous: true } },
  { path: '', redirectTo: '', pathMatch: 'full' }
];

routes.forEach(route => {
  if (!route.data?.['allowAnonymous']) {
    route.canActivate = RouteGuard;
  }
});
