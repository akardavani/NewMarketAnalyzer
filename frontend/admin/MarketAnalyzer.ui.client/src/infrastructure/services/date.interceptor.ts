import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { convertDates } from "../utils";

@Injectable()
export class DateInterceptor implements HttpInterceptor {

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.body) {
      const body = convertDates(req.body);
      const clone = req.clone({ body });
      return next.handle(clone);
    }
    else {
      return next.handle(req);
    }
  }

}
