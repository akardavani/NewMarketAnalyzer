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
  private readonly marketSettingsCache = new Map<string, SettingItem[]>();
  readonly providerSettingDraft: SettingItem = { scope: '', key: '', value: '' };
  readonly jobs = [
    { id: 1, capability: 1, label: 'همگام‌سازی نمادها' },
    { id: 2, capability: 2, label: 'دریافت کندل تاریخی' },
    { id: 3, capability: 3, label: 'وضعیت لحظه‌ای' },
    { id: 4, capability: 4, label: 'دفتر سفارش' },
    { id: 5, capability: 6, label: 'فعالیت حقیقی/حقوقی' },
    { id: 6, capability: 7, label: 'NAV صندوق‌ها' }
  ];

  ngOnInit(): void { this.reload(); }

  reload(): void {
    this.loading.set(true);
    this.error.set('');
    this.service.overview().subscribe({
      next: result => { this.marketSettingsCache.clear(); this.markets.set(result); this.loading.set(false); },
      error: e => { this.error.set(this.message(e)); this.loading.set(false); }
    });
  }

  marketSettings(market: MarketOverview): SettingItem[] {
    const cached = this.marketSettingsCache.get(market.key);
    if (cached) return cached;

    const settings = Object.entries(market.settings ?? {})
      // These are per-symbol retry/cache markers stored internally in MarketSettings,
      // not operator configuration. Showing them creates hundreds of cryptic rows.
      .filter(([key]) => !key.toLowerCase().startsWith('provider:'))
      .filter(([key]) => !/^Ingestion:NoDataUntil:\d+$/i.test(key) && !/^NavRetry:\d+$/i.test(key))
      .map(([key, value]) => ({ scope: `market:${market.key}`, key, value }));
    this.marketSettingsCache.set(market.key, settings);
    return settings;
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

  jobsFor(market: MarketOverview): Array<{ id: number; capability: number; label: string }> {
    const availableCapabilities = new Set(market.capabilities.map(capability => capability.capability));
    // Keep the three common market operations visible for every market. A market
    // without a provider for one of them will show the option disabled in the UI.
    return this.jobs.filter(job => job.id <= 3 || availableCapabilities.has(job.capability));
  }

  isJobAvailable(market: MarketOverview, job: { capability: number }): boolean {
    const capability = market.capabilities.find(item => item.capability === job.capability);
    if (capability?.activeProviderKey) return true;

    // Markets without a live quote provider can still refresh the current-state
    // snapshot from their active candle provider when the operator runs it manually.
    if (job.capability === 3) {
      return !!market.capabilities.find(item => item.capability === 2)?.activeProviderKey;
    }

    return false;
  }

  jobOptionLabel(market: MarketOverview, job: { capability: number; label: string }): string {
    if (job.capability === 3) {
      const hasLiveProvider = !!market.capabilities.find(item => item.capability === 3)?.activeProviderKey;
      if (!hasLiveProvider && this.isJobAvailable(market, job)) return `${job.label} (از آخرین کندل)`;
    }
    if (this.isJobAvailable(market, job)) return job.label;
    return `${job.label} (منبع این کار برای بازار تنظیم نشده)`;
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
        IntervalSeconds: 'فاصلهٔ اجرا (دقیقه)',
        RunAt: 'ساعت اجرای روزانه',
        WindowFrom: 'شروع ساعت دریافت',
        WindowTo: 'پایان ساعت دریافت'
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

  isIntervalSetting(setting: SettingItem): boolean {
    return /^Scheduler:[^:]+:IntervalSeconds$/i.test(setting.key);
  }

  intervalMinutes(setting: SettingItem): number {
    const seconds = Number(setting.value);
    return Number.isFinite(seconds) ? seconds / 60 : 1;
  }

  setIntervalMinutes(setting: SettingItem, value: number | string): void {
    const minutes = Number(value);
    if (Number.isFinite(minutes) && minutes > 0)
      setting.value = String(Math.round(minutes * 60));
  }

  settingInputType(setting: SettingItem): string {
    if (this.isSecret(setting.key)) return 'password';
    if (/^Scheduler:[^:]+:(RunAt|WindowFrom|WindowTo)$/i.test(setting.key)) return 'time';
    return 'text';
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

  saveMarketSettings(market: MarketOverview): void {
    const settings = this.marketSettings(market)
      .filter(setting => !this.isSecret(setting.key))
      .map(({ key, value }) => ({ key, value }));
    this.perform(
      () => this.service.setSettings(`market:${market.key}`, settings),
      `تنظیمات ${market.persianName || market.name} ذخیره شد.`
    );
  }

  setMarketActive(market: MarketOverview): void {
    const isActive = !market.isActive;
    const state = isActive ? 'فعال' : 'غیرفعال';
    this.perform(
      () => this.service.setMarketActive(market.key, isActive),
      `دریافت دادهٔ ${market.persianName || market.name} ${state} شد.`
    );
  }

  saveProviderSettings(): void {
    const providerKey = this.selectedProvider();
    const settings = this.providerSettings()
      .filter(setting => !this.isSecret(setting.key))
      .map(({ key, value }) => ({ key, value }));
    this.perform(
      () => this.service.setSettings(`provider:${providerKey}`, settings),
      `تنظیمات ${providerKey} ذخیره شد.`
    );
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

  isBoolean(setting: SettingItem): boolean {
    return ['true', 'false'].includes(setting.value.trim().toLowerCase());
  }

  booleanValue(setting: SettingItem): boolean { return setting.value.trim().toLowerCase() === 'true'; }

  setBoolean(setting: SettingItem, event: Event): void {
    setting.value = (event.target as HTMLInputElement).checked ? 'true' : 'false';
  }

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
