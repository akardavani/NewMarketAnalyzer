import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { AppDataService } from './app-data.service';
import { AppState } from '@typings';


function routeGuard(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
  const promise = new Promise<boolean>((resolve, reject) => {
    const authService = inject(AuthService);
    if (state.root.data['allowAnonymous']) {
      resolve(true);
    }
    else if (authService.authenticated) {
      const appData = inject(AppDataService);
      if (appData.appState == AppState.Ready || state.url == '/setup') {
        resolve(true);
      }
      else {
        const router = inject(Router);
        router.navigate(['setup']);
        reject('Setup required');
      }
    }
    else {
      authService.challenge(state.url);
      reject('Authentication required');
    }
  });

  return promise;
}

export const RouteGuard: CanActivateFn[] = [routeGuard];
