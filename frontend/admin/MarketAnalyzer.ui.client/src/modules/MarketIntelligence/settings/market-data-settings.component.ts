import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MarketCapability, MarketDataSettingsService, MarketOverview, MarketJobResult, SettingItem } from './market-data-settings.service';

@Component({
  selector: 'mi-market-data-settings',
  imports: [CommonModule, FormsModule],
  templateUrl: './market-data-settings.component.html',
  styleUrl: './market-data-settings.component.scss'
})
export class MarketDataSettingsComponent implements OnInit {
  private readonly service = inject(MarketDataSettingsService);
  readonly markets = signal<MarketOverview[]>([]);
  readonly providerSettings = signal<SettingItem[]>([]);
  readonly selectedProvider = signal('');
  readonly busy = signal(false);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly notice = signal('');
  readonly lastJob = signal<MarketJobResult | null>(null);
  readonly marketSettingDrafts: Record<string, SettingItem> = {};
  readonly providerSettingDraft: SettingItem = { scope: '', key: '', value: '' };
  readonly jobs = [
    { id: 1, label: 'همگام‌سازی نمادها' },
    { id: 2, label: 'دریافت کندل تاریخی' },
    { id: 3, label: 'وضعیت لحظه‌ای' },
    { id: 4, label: 'دفتر سفارش' },
    { id: 5, label: 'فعالیت حقیقی/حقوقی' },
    { id: 6, label: 'NAV صندوق‌ها' }
  ];

  ngOnInit(): void { this.reload(); }

  reload(): void {
    this.loading.set(true);
    this.error.set('');
    this.service.overview().subscribe({
      next: result => { this.markets.set(result); this.loading.set(false); },
      error: e => { this.error.set(this.message(e)); this.loading.set(false); }
    });
  }

  marketSettings(market: MarketOverview): SettingItem[] {
    return Object.entries(market.settings ?? {})
      .filter(([key]) => !key.toLowerCase().startsWith('provider:'))
      .map(([key, value]) => ({ scope: `market:${market.key}`, key, value }));
  }

  marketSettingDraft(marketKey: string): SettingItem {
    return this.marketSettingDrafts[marketKey] ??= { scope: `market:${marketKey}`, key: '', value: '' };
  }

  addMarketSetting(market: MarketOverview): void {
    const draft = this.marketSettingDraft(market.key);
    this.addSetting(draft, () => { draft.key = ''; draft.value = ''; });
  }

  addProviderSetting(): void {
    this.providerSettingDraft.scope = `provider:${this.selectedProvider()}`;
    this.addSetting(this.providerSettingDraft, () => {
      this.providerSettingDraft.key = '';
      this.providerSettingDraft.value = '';
      this.selectProvider(this.selectedProvider());
    });
  }

  private addSetting(draft: SettingItem, reset: () => void): void {
    if (!draft.scope.split(':')[1] || !draft.key.trim()) {
      this.error.set('کلید تنظیم را وارد کن.');
      return;
    }
    this.perform(() => this.service.setSetting(draft.scope, draft.key.trim(), draft.value), 'تنظیم جدید ذخیره شد.', reset);
  }

  capabilityName(id: number): string {
    return ({ 1: 'کاتالوگ نماد', 2: 'کندل تاریخی', 3: 'وضعیت لحظه‌ای', 4: 'دفتر سفارش', 5: 'رویداد شرکتی', 6: 'فعالیت حقیقی/حقوقی', 7: 'NAV صندوق' } as Record<number, string>)[id] ?? `قابلیت ${id}`;
  }

  settingLabel(key: string): string {
    const scheduler = /^Scheduler:([^:]+):([^:]+)$/i.exec(key);
    if (scheduler) {
      const jobs: Record<string, string> = {
        SyncCatalog: 'همگام‌سازی فهرست نمادها',
        IngestCandles: 'دریافت کندل روزانه',
        RefreshCurrentState: 'به‌روزرسانی وضعیت لحظه‌ای',
        RefreshOrderBook: 'به‌روزرسانی دفتر سفارش',
        RefreshInvestorActivity: 'دریافت فعالیت حقیقی و حقوقی',
        RefreshFundNav: 'دریافت NAV صندوق‌ها'
      };
      const parts: Record<string, string> = {
        Enabled: 'فعال بودن کار',
        IntervalSeconds: 'فاصلهٔ اجرا (ثانیه)',
        RunAt: 'ساعت اجرای روزانه',
        WindowFrom: 'شروع بازهٔ اجرا',
        WindowTo: 'پایان بازهٔ اجرا'
      };
      return `${jobs[scheduler[1]] ?? scheduler[1]} — ${parts[scheduler[2]] ?? scheduler[2]}`;
    }

    return ({
      BaseUrl: 'نشانی سرویس',
      ApiUrl: 'نشانی API',
      TimeoutSeconds: 'مهلت پاسخ (ثانیه)',
      PageSize: 'تعداد در هر درخواست'
    } as Record<string, string>)[key] ?? key;
  }

  providerChanged(market: MarketOverview, capability: MarketCapability, providerKey: string): void {
    if (!providerKey || providerKey === capability.activeProviderKey) return;
    const confirmWipe = capability.capability === 2 && window.confirm('با تغییر منبع کندل، تاریخچهٔ فعلی این بازار پاک و با منبع تازه دریافت می‌شود. ادامه می‌دهید؟');
    if (capability.capability === 2 && !confirmWipe) { this.reload(); return; }

    this.perform(() => this.service.switchProvider(market.key, capability.capability, providerKey, confirmWipe), 'منبع فعال تغییر کرد.');
  }

  selectProvider(providerKey: string): void {
    this.selectedProvider.set(providerKey);
    this.providerSettings.set([]);
    if (!providerKey) return;
    this.service.providerSettings(providerKey).subscribe({
      next: result => this.providerSettings.set(result),
      error: e => this.error.set(this.message(e))
    });
  }

  saveSetting(item: SettingItem): void {
    this.perform(() => this.service.setSetting(item.scope, item.key, item.value), 'تنظیم ذخیره شد.');
  }

  runJob(market: MarketOverview, jobId: number): void {
    this.lastJob.set(null);
    this.perform(() => this.service.runJob(market.key, jobId), 'اجرای دستی پایان یافت.', result => this.lastJob.set(result as MarketJobResult));
  }

  refreshCatalogs(): void {
    this.busy.set(true);
    this.error.set('');
    this.notice.set('');
    this.service.refreshCatalogs().subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'market-catalogs.zip';
        document.body.appendChild(link);
        link.click();
        link.remove();
        setTimeout(() => URL.revokeObjectURL(url), 0);
        this.notice.set('همگام‌سازی کاتالوگ‌ها پایان یافت و فایل JSON دریافت شد. برای دیدن خطاهای هر بازار، refresh-report.json داخل ZIP را ببین.');
        this.busy.set(false);
        this.reload();
      },
      error: e => { this.error.set(this.message(e)); this.busy.set(false); }
    });
  }

  isSecret(key: string): boolean { return /password|secret|token|apikey|username|credential/i.test(key); }

  private perform<T>(request: () => import('rxjs').Observable<T>, success: string, done?: (result: T) => void): void {
    this.busy.set(true);
    this.error.set('');
    this.notice.set('');
    request().subscribe({
      next: result => { done?.(result); this.notice.set(success); this.busy.set(false); this.reload(); },
      error: e => { this.error.set(this.message(e)); this.busy.set(false); }
    });
  }

  private message(error: any): string {
    return error?.error?.errors?.join(' ') || error?.message || 'خطای نامشخص در ارتباط با سرور';
  }
}
