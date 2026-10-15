import { inject, LOCALE_ID, provideAppInitializer, provideZonelessChangeDetection } from '@angular/core';
import { AppComponent } from './app/app.component';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { bootstrapApplication } from '@angular/platform-browser';
import { PreloadAllModules, provideRouter, withPreloading } from '@angular/router';
import { routes } from './app/app.routes';
import { AuthInterceptor } from '@services/auth.interceptor';
import { AppHttpInterceptor } from '@services/http.interceptor';
import { CultureInfo } from '@xobin/globalization';
import { cultureData as faCultureData } from "@xobin/globalization/dist/locales/fa-IR";
import { AppDataService } from './infrastructure/services/app-data.service';
import { CAPTCHA_OPTIONS } from '@components/captcha/captcha.component';
import { DateInterceptor } from '@services/date.interceptor';
import { ThemeService } from '@services/theme.service';
import { BrandService } from '@services/brand.service';

CultureInfo.loadCultureInfo('fa-IR', faCultureData);
var faCulture = new CultureInfo('fa-IR');
CultureInfo.currentCulture = faCulture;
CultureInfo.currentUICulture = faCulture;

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideAppInitializer(() => {
      inject(ThemeService).init();
      inject(BrandService).init();
    }),
    provideAppInitializer(() => {
      const service = inject(AppDataService);
      return service.init();
    }),

    provideZonelessChangeDetection(),
    provideRouter(routes, withPreloading(PreloadAllModules)),

    { provide: LOCALE_ID, useValue: 'fa-IR' },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: DateInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AppHttpInterceptor, multi: true },
    { provide: CAPTCHA_OPTIONS, useValue: { url: appSettings.captchaUrl, lifetime: appSettings.captchaLifetime } }
  ]
}).catch(err => console.error(err));
