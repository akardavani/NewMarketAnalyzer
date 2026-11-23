import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { AppDataService } from './app-data.service';
import { AppState } from '@typings';


async function routeGuard(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
  const authService = inject(AuthService);
  if (state.root.data['allowAnonymous']) {
    return true;
  }

  if (!authService.authenticated) {
    authService.challenge(state.url);
    return false;
  }

  const appData = inject(AppDataService);
  const router = inject(Router);
  await appData.init();

  if (appData.appState() === AppState.Ready || state.url === '/setup') {
    return true;
  }

  await router.navigate(['setup']);
  return false;
}

export const RouteGuard: CanActivateFn[] = [routeGuard];
