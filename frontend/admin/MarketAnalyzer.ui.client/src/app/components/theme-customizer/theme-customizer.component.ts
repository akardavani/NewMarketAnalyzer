import { Component, computed, HostListener, inject } from '@angular/core';
import { ShellStateService } from '../../../infrastructure/services/shell-state.service';
import {
  ThemeDensity, ThemeMode, ThemePalette, ThemeService, ThemeSettings, ThemeSubmit, ThemeTypeface, THEME_OPTIONS
} from '../../../infrastructure/services/theme.service';

interface Option<T extends string> { value: T; label: string; }

/** پنل شخصی‌سازی ظاهر؛ فقط ThemeService.update() را صدا می‌زند و هیچ رنگی را مستقیم نمی‌نویسد. */
@Component({
  selector: 'fd-theme-customizer',
  imports: [],
  templateUrl: './theme-customizer.component.html',
  styleUrl: './theme-customizer.component.scss'
})
export class ThemeCustomizerComponent {
  private readonly theme = inject(ThemeService);
  protected readonly shell = inject(ShellStateService);

  protected readonly settings = this.theme.settings;

  protected readonly modes: Option<ThemeMode>[] = [
    { value: 'light', label: 'روشن' },
    { value: 'dark', label: 'تیره' },
    { value: 'system', label: 'سیستم' }
  ];
  protected readonly palettes: Option<ThemePalette>[] = [
    { value: 'violet', label: 'بنفش' },
    { value: 'blue', label: 'آبی' },
    { value: 'emerald', label: 'سبز' },
    { value: 'rose', label: 'صورتی' },
    { value: 'orange', label: 'نارنجی' },
    { value: 'slate', label: 'خاکستری' }
  ].filter(o => (THEME_OPTIONS.palette as readonly string[]).includes(o.value)) as Option<ThemePalette>[];
  protected readonly densities: Option<ThemeDensity>[] = [
    { value: 'compact', label: 'فشرده' },
    { value: 'normal', label: 'عادی' },
    { value: 'comfy', label: 'راحت' }
  ];
  protected readonly typefaces: Option<ThemeTypeface>[] = [
    { value: 'iransans', label: 'ایران‌سنس' },
    { value: 'iransans-farsi', label: 'ایران‌سنس (ارقام فارسی)' }
  ];
  protected readonly submits: Option<ThemeSubmit>[] = [
    { value: 'theme', label: 'هم‌رنگ تم' },
    { value: 'green', label: 'سبز' },
    { value: 'blue', label: 'آبی' },
    { value: 'rose', label: 'قرمز' }
  ];

  protected readonly isOpen = computed(() => this.shell.customizerOpen());

  protected set<K extends keyof ThemeSettings>(key: K, value: ThemeSettings[K]): void {
    this.theme.update({ [key]: value });
  }

  protected reset(): void {
    this.theme.reset();
  }

  protected close(): void {
    this.shell.customizerOpen.set(false);
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.isOpen()) {
      this.close();
    }
  }
}
