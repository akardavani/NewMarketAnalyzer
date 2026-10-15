import { DOCUMENT } from '@angular/common';
import { inject, Injectable, signal } from '@angular/core';

/** اگر این کلید را تغییر دهید وضعیت قبلی کاربران نادیده گرفته می‌شود. */
const STORAGE_KEY = 'fd.shell.v1';

/**
 * وضعیت پوستهٔ برنامه: سایدبار باز/بسته (در مرورگر به‌خاطر سپرده می‌شود) و پنل شخصی‌سازی.
 * رفتار مثل Flux: با دکمه باز می‌شود و می‌ماند؛ با دکمه بسته می‌شود و می‌ماند.
 */
@Injectable({ providedIn: 'root' })
export class ShellStateService {
  private readonly win = inject(DOCUMENT).defaultView;

  /** true = سایدبار فقط آیکون (ریل). */
  readonly collapsed = signal<boolean>(this.read());
  readonly customizerOpen = signal(false);
  /** جستجوی سریع (Ctrl K). */
  readonly searchOpen = signal(false);

  toggleSidebar(): void {
    this.setCollapsed(!this.collapsed());
  }

  setCollapsed(value: boolean): void {
    this.collapsed.set(value);
    try {
      this.win?.localStorage.setItem(STORAGE_KEY, JSON.stringify({ collapsed: value }));
    } catch {
      // storage مسدود: فقط برای همین نشست.
    }
  }

  private read(): boolean {
    try {
      const raw = this.win?.localStorage.getItem(STORAGE_KEY);
      return raw ? JSON.parse(raw)?.collapsed === true : false;
    } catch {
      return false;
    }
  }
}
