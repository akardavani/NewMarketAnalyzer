import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { PlainObject } from '../typings';

const StatusCodeRedirectPath: PlainObject<string> = {
  401: '/login',
  403: '/forbidden'
};

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private router: Router, private authService: AuthService) {

  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let headers: HttpHeaders = request.headers.set('Authorization', `Bearer ${this.authService.accessToken}`);
    request = request.clone({ headers });

    return next
      .handle(request)
      .pipe(
        catchError(response => {
          if (response instanceof HttpErrorResponse) {
            const redirectPath = StatusCodeRedirectPath[response.status];
            if (redirectPath) {
              this.router.navigate([redirectPath]);
            }
          }

          return throwError(() => response);
        })
      );

  }

}
