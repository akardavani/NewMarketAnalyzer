import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from '../jwt_decode';
import { PlainObject, AuthResult, AuthData, Nullable } from '../typings';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {

  accessToken: Nullable<string> = null;
  user: PlainObject<string> = {};

  private _expireDate: Nullable<Date> = null;

  constructor(
    private readonly router: Router,
    private readonly storage: TokenStorageService) {
    this.initAuthentication();
  }

  get authenticated(): boolean {
    return !!this.accessToken && !this.expired;
  }

  get expired(): boolean {
    if (!this._expireDate) {
      return true;
    }

    const currentDate = new Date();
    return currentDate > this._expireDate;
  }

  getUserClaim(key: string, defaultValue: string = ''): string {
    return (this.user && this.user[key]) || defaultValue;
  }

  challenge(returnUrl: string | undefined): void {
    if (returnUrl) {
      sessionStorage.setItem('redirectUrl', returnUrl);
    }

    this.router.navigate(['login'], {
      queryParams: { returnUrl }, queryParamsHandling: 'merge'
    });
  }

  signIn(tokens: AuthResult): void {
    this.initAuthentication({
      accessToken: tokens.accessToken,
      expireDate: Date.now() + tokens.lifetime
    });

    const redirectUrl = sessionStorage.getItem('redirectUrl') || '';
    sessionStorage.removeItem('redirectUrl');
    window.location.replace(redirectUrl);
  }

  signOut(): void {
    this.storage.remove();
    this.user = {};
    this._expireDate = null;
    this.accessToken = null;

    location.assign(`${location.protocol}//${location.host}/login`);
  }

  private initAuthentication(authData?: AuthData): void {
    const mustStore = !!authData;
    authData = authData || this.storage.get();
    if (!authData) {
      return;
    }

    this._expireDate = new Date(authData.expireDate);
    if (!this.expired) {
      this.accessToken = authData.accessToken;
      this.user = jwtDecode(authData.accessToken);
    }

    if (mustStore) {
      this.storage.set(authData);
    }
  }

}


