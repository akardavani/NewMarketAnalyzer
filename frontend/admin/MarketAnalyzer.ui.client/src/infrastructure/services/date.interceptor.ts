import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { convertDates } from "../utils";

@Injectable()
export class DateInterceptor implements HttpInterceptor {


  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // فایل‌های آپلودی نباید وارد تبدیل تاریخ شوند.
    if (req.body instanceof FormData) {
      return next.handle(req);
    }

    if (req.body) {
      const body = convertDates(req.body);
      const clone = req.clone({ body });
      return next.handle(clone);
    }

    return next.handle(req);
  }


}
