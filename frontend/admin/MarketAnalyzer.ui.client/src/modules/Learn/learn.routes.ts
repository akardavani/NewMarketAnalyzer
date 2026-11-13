import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'overview' },
  { path: 'overview', title: 'نمای کلی یادگیری', loadComponent: () => import('./pages/overview/learn-overview.component').then(m => m.LearnOverviewComponent) },
  { path: 'collections', title: 'مجموعه‌های یادگیری', loadComponent: () => import('./pages/collections/learn-collections.component').then(m => m.LearnCollectionsComponent) },
  { path: 'collections/:id', title: 'محتوای مجموعه یادگیری', loadComponent: () => import('./pages/collection/learn-collection.component').then(m => m.LearnCollectionComponent) }
];
