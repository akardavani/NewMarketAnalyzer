import { Routes } from '@angular/router';
import { UnknownLoginHistoryComponent } from './unknown-login-history/unknown-login-history.component';
import { UsersLoginHistoryComponent } from './users-login-history/users-login-history.component';
import { UsersComponent } from './users/users/users.component';
import { RolesComponent } from './roles/roles/roles.component';

export const routes: Routes = [
  { path: 'roles', title: 'نقش‌ها', component: RolesComponent },
  { path: 'users', title: 'کاربران', component: UsersComponent },
  { path: 'users-login-history', title: 'گزارش ورود و خروج کاربران', component: UsersLoginHistoryComponent },
  { path: 'unknown-login-history', title: 'گزارش ورود و خروج ناشناس', component: UnknownLoginHistoryComponent },
];
