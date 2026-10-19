import { Routes } from '@angular/router';

/** Add a new strategy = add one folder under ./strategies and one line here. */
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'ratio-candles' },
  { path: 'iran-equity-main', title: 'سهم‌های برتر بازار', loadComponent: () => import('./strategies/iran-equity-main/iran-equity-main.component').then(m => m.IranEquityMainComponent) },
  { path: 'bottom-reversal', title: 'برگشت از کف', loadComponent: () => import('./strategies/bottom-reversal/bottom-reversal.component').then(m => m.BottomReversalComponent) },
  { path: 'iran-queue', title: 'صف خرید', loadComponent: () => import('./strategies/iran-queue/iran-queue.component').then(m => m.IranQueueComponent) },
  { path: 'multi-indicator-divergence', title: 'واگرایی اندیکاتورها', loadComponent: () => import('./strategies/multi-indicator-divergence/multi-indicator-divergence.component').then(m => m.MultiIndicatorDivergenceComponent) },
  { path: 'ratio-candles', title: 'نسبت نمادها', loadComponent: () => import('./strategies/ratio-candles/ratio-candles.component').then(m => m.RatioCandlesComponent) },
  { path: 'ytm-future', title: 'YTM', loadComponent: () => import('./strategies/ytm-future/ytm-future.component').then(m => m.YtmFutureComponent) }
];
