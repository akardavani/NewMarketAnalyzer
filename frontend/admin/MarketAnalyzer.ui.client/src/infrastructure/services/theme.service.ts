import { DOCUMENT } from '@angular/common';
import { computed, inject, Injectable, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark' | 'system';
export type ThemePalette = 'violet' | 'blue' | 'emerald' | 'rose' | 'orange' | 'slate';
export type ThemeDensity = 'compact' | 'normal' | 'comfy';
export type ThemeTypeface = 'iransans' | 'iransans-farsi';
/** رنگ دکمه‌های «ثبت»: theme = هم‌رنگ دکمهٔ اصلی */
export type ThemeSubmit = 'theme' | 'green' | 'blue' | 'rose';

export interface ThemeSettings {
  mode: ThemeMode;
  palette: ThemePalette;
  density: ThemeDensity;
  typeface: ThemeTypeface;
  submit: ThemeSubmit;
}

/**
 * مقادیر پیش‌فرض هر محصول. برای BackOffice / سبدگردانی فقط palette را عوض کنید.
 */
export const THEME_DEFAULTS: Readonly<ThemeSettings> = {
  mode: 'dark',
  palette: 'violet',
  density: 'normal',
  typeface: 'iransans',
  submit: 'theme'
};

/** گزینه‌های مجاز؛ مقدار ذخیره‌شده‌ای که در این فهرست نباشد نادیده گرفته می‌شود. */
export const THEME_OPTIONS = {
  mode: ['light', 'dark', 'system'],
  palette: ['violet', 'blue', 'emerald', 'rose', 'orange', 'slate'],
  density: ['compact', 'normal', 'comfy'],
  typeface: ['iransans', 'iransans-farsi'],
  submit: ['theme', 'green', 'blue', 'rose']
} as const satisfies { [K in keyof ThemeSettings]: readonly ThemeSettings[K][] };

/** اگر این کلید را تغییر دهید ترجیح قبلی کاربران نادیده گرفته می‌شود. */
const STORAGE_KEY = 'fd.theme.v1';

/**
 * تنها مسئول اعمال تم: attribute های `<html>` را تنظیم می‌کند و ترجیح کاربر را در مرورگر نگه می‌دارد.
 * همهٔ رنگ‌ها/فونت‌ها در src/styles/theme/ تعریف شده‌اند؛ این سرویس فقط انتخاب می‌کند.
 */
@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly root = inject(DOCUMENT).documentElement;
  private readonly win = inject(DOCUMENT).defaultView;
  private readonly systemDark = signal(false);
  private initialized = false;

  readonly settings = signal<ThemeSettings>({ ...THEME_DEFAULTS });

  /** حالت واقعی (system به light/dark تبدیل شده). */
  readonly resolvedMode = computed<'light' | 'dark'>(() => {
    const mode = this.settings().mode;
    return mode === 'system' ? (this.systemDark() ? 'dark' : 'light') : mode;
  });

  /**
   * با هر بار اعمال تم یکی زیاد می‌شود. کامپوننت‌هایی که رنگ را از JS می‌خوانند (نمودارها، AG Grid)
   * می‌توانند در effect() به آن وابسته شوند و دوباره رسم کنند.
   */
  readonly version = signal(0);

  /** در APP_INITIALIZER صدا زده می‌شود (پیش از اولین رندر). */
  init(): void {
    if (this.initialized) {
      return;
    }
    this.initialized = true;

    const media = this.win?.matchMedia?.('(prefers-color-scheme: dark)');
    if (media) {
      this.systemDark.set(media.matches);
      media.addEventListener('change', e => {
        this.systemDark.set(e.matches);
        this.apply();
      });
    }

    this.settings.set(this.read());
    this.apply();
  }

  /** تغییر یک یا چند مورد؛ همان لحظه اعمال و ذخیره می‌شود. */
  update(patch: Partial<ThemeSettings>): void {
    this.settings.update(current => this.sanitize({ ...current, ...patch }));
    this.apply();
    this.write();
  }

  /** بازگشت به پیش‌فرض محصول. */
  reset(): void {
    this.settings.set({ ...THEME_DEFAULTS });
    this.apply();
    this.write();
  }

  /** رنگ/مقدار یک توکن CSS را به‌صورت رشته می‌خواند، مثلاً cssVar('--fd-primary'). */
  cssVar(name: string): string {
    return this.win?.getComputedStyle(this.root).getPropertyValue(name).trim() ?? '';
  }

  /**
   * رنگ یک توکن را به رشتهٔ رنگ استاندارد (hex / rgb) تبدیل می‌کند؛ برای کتابخانه‌هایی که
   * سینتکس مدرن CSS (rgb با فاصله، color-mix) را نمی‌فهمند، مثل CanvasJS.
   */
  cssColor(name: string): string {
    const raw = this.cssVar(name);
    const ctx = this.root.ownerDocument.createElement('canvas').getContext('2d');
    if (!ctx || !raw) {
      return raw;
    }
    const sentinel = '#010203';
    ctx.fillStyle = sentinel;
    ctx.fillStyle = raw;
    const out = String(ctx.fillStyle);
    return out === sentinel ? raw : out;
  }

  private apply(): void {
    const s = this.settings();
    const mode = this.resolvedMode();
    const ds = this.root.dataset;

    ds['mode'] = mode;
    ds['palette'] = s.palette;
    ds['density'] = s.density;
    ds['typeface'] = s.typeface;
    ds['submit'] = s.submit;
    // Bootstrap 5.3 (color-mode-type: data)
    this.root.setAttribute('data-bs-theme', mode);

    this.version.update(v => v + 1);
  }

  private read(): ThemeSettings {
    try {
      const raw = this.win?.localStorage.getItem(STORAGE_KEY);
      return this.sanitize(raw ? JSON.parse(raw) : {});
    } catch {
      return { ...THEME_DEFAULTS };
    }
  }

  private write(): void {
    try {
      this.win?.localStorage.setItem(STORAGE_KEY, JSON.stringify(this.settings()));
    } catch {
      // حالت private یا storage مسدود: تم فقط برای همین نشست می‌ماند.
    }
  }

  /** هر کلید نامعتبر یا نبوده به پیش‌فرض برمی‌گردد. */
  private sanitize(input: Partial<Record<keyof ThemeSettings, unknown>>): ThemeSettings {
    const pick = <K extends keyof ThemeSettings>(key: K): ThemeSettings[K] => {
      const allowed = THEME_OPTIONS[key] as readonly unknown[];
      const value = input?.[key];
      return (allowed.includes(value) ? value : THEME_DEFAULTS[key]) as ThemeSettings[K];
    };
    return {
      mode: pick('mode'),
      palette: pick('palette'),
      density: pick('density'),
      typeface: pick('typeface'),
      submit: pick('submit')
    };
  }
}
