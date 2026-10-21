import {
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpEvent,
  HttpResponse,
  HttpHeaders,
  HttpErrorResponse
} from '@angular/common/http';
import { Injectable } from '@angular/core'
import { Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { GrowlService } from './growl.service';
import { isApiResult } from '../typings';
import { bind } from '@utils';

const RX_DATE = /^\/D\((\d+)\)\/$/;

@Injectable()
export class AppHttpInterceptor implements HttpInterceptor {

  constructor(private readonly growl: GrowlService) {

  }




  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let headers = request.headers;

    if (request.body instanceof FormData) {
      // اجازه بده مرورگر Content-Type و boundary را تنظیم کند.
      headers = headers.delete('Content-Type');

      request = request.clone({ headers });
    } else if (request.responseType === 'json') {
      headers = headers.set('Accept', 'application/json');
      headers = headers.set('Content-Type', 'application/json');

      request = request.clone({
        responseType: 'text',
        headers
      });
    } else {
      request = request.clone({ headers });
    }

    return next
      .handle(request)
      .pipe(
        map(event => {
          if (event instanceof HttpResponse && typeof event.body === 'string') {
            const parsed = JSON.parse(event.body, parseDate);
            return event.clone({ body: parsed });
          }
          return event;
        }),
        tap(this.tapResponse),
        catchError(this.handleError)
      );
  }

  @bind()
  private tapResponse(response: HttpEvent<any>) {
    if (response instanceof HttpResponse) {
      this.showErrors(response.body);
    }
  }

  @bind()
  private handleError(response: any): Observable<any> {
    if (response instanceof HttpErrorResponse) {
      const result = typeof (response.error) === 'string' ?
        JSON.parse(response.error) :
        response.error;
      if (!this.showErrors(result)) {
        this.growl.error({ timeout: 5000, content: 'درخواست شما با خطا مواجه شد، پارامترهای ارسالی خود را بررسی کنید.' });
      }
    }

    return throwError(() => response);
  }

  private showErrors(response: any): boolean {
    if (isApiResult(response) && !response.successed && response.errors) {
      response.errors.forEach(err => this.growl.error({ timeout: 10000, content: err }));
      return true;
    }

    return false;
  }

}

function parseDate(_: string, value: any): any {
  if (typeof value === 'string' && RX_DATE.test(value)) {
    //const time = RX_DATE.exec(value);
    return new Date(+value.replace(RX_DATE, '$1'));
  }

  return value;
}
