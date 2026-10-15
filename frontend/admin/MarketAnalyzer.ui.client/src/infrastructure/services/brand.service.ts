import { DOCUMENT } from '@angular/common';
import { inject, Injectable } from '@angular/core';

/**
 * نام محصول، شرکت و نسخه — از `public/env.js` (window.appSettings) خوانده می‌شود، نه از کد.
 * هر محصول (صندوق / BackOffice / سبدگردانی) فقط env.js خودش را عوض می‌کند.
 * در قالب‌ها (layout، login، forgot/reset password) به‌جای متن ثابت از این سرویس استفاده کنید.
 */
@Injectable({ providedIn: 'root' })
export class BrandService {
  private readonly document = inject(DOCUMENT);

  /** عنوان محصول، مثلاً «صندوق مبین سرمایه». */
  readonly productName: string = appSettings.productName?.trim() || appSettings.appName?.trim() || 'سامانه';
  /** نام کامل برای صفحات ورود؛ پیش‌فرض: productName. */
  readonly productFullName: string = appSettings.productFullName?.trim() || this.productName;
  /** نام مدیر/سبدگردان زیر عنوان صفحات ورود؛ خالی = مخفی. */
  readonly managerName: string = appSettings.managerName?.trim() || '';
  /** نام شرکت سازنده، مثلاً «پویا اندیشان رسام رایان». */
  readonly companyName: string = appSettings.companyName?.trim() || '';
  /** آدرس لوگو (اختیاری). خالی = لوگوی پیش‌فرض (کلاس .broker-logo). */
  readonly logoUrl: string = appSettings.logoUrl?.trim() || '';
  readonly version: string = appSettings.version ?? '';
  /** دکمهٔ میان‌بر هدر؛ path خالی = مخفی. */
  readonly quickAction = {
    label: appSettings.quickActionLabel?.trim() || 'ایجاد',
    path: appSettings.quickActionPath?.trim() || ''
  };

  /** در APP_INITIALIZER صدا زده می‌شود. */
  init(): void {
    this.document.title = this.productName;
  }
}
