import { Injectable } from '@angular/core';
import { AuthData } from '../typings';

const AUTH_STORAGE_KEY = 'auth_data';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {

  constructor() { }

  get(): AuthData | undefined {
    const data = localStorage.getItem(AUTH_STORAGE_KEY);
    return data ? JSON.parse(data) : undefined;
  }

  set(authData: AuthData): void {
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(authData));
  }

  remove(): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
  }

}
