import { ChangeDetectionStrategy, Component, HostListener, OnDestroy, computed, inject, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { DuplicateFilesService, DuplicateScanResult } from './duplicate-files.service';

@Component({
  selector: 'fd-duplicate-files-modal',
  standalone: true,
  templateUrl: './duplicate-files-modal.component.html',
  styleUrl: './duplicate-files-modal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DuplicateFilesModalComponent implements OnDestroy {
  private readonly service = inject(DuplicateFilesService);
  private request?: Subscription;
  readonly open = signal(false);
  readonly paths = signal('');
  readonly recursive = signal(true);
  readonly busy = signal(false);
  readonly error = signal('');
  readonly result = signal<DuplicateScanResult | null>(null);
  readonly page = signal(0);
  readonly visibleGroups = computed(() => this.result()?.groups.slice(this.page() * 50, (this.page() + 1) * 50) ?? []);
  readonly pages = computed(() => Math.ceil((this.result()?.groups.length ?? 0) / 50));

  scan(): void {
    if (this.busy()) return;
    const paths = [...new Set(this.paths().split(/\r?\n/).map(p => p.trim()).filter(Boolean))];
    if (!paths.length || paths.length > 20) { this.error.set('بین ۱ تا ۲۰ مسیر، هر مسیر در یک خط وارد کنید.'); return; }
    this.error.set(''); this.result.set(null); this.page.set(0); this.busy.set(true);
    this.request = this.service.scan(paths, this.recursive()).subscribe({
      next: result => { this.result.set(result); this.busy.set(false); },
      error: err => {
        this.busy.set(false);
        this.error.set(err.status === 404 ? 'ابزار در سرور فعال نشده یا مسیرهای مجاز تنظیم نشده‌اند.' :
          err.status === 403 ? 'مجوز دسترسی به این ابزار را ندارید.' :
          err.error?.message || err.message || 'خطا در ارتباط با سرور');
      }
    });
  }
  cancel(): void {
    this.request?.unsubscribe(); this.request = undefined;
    if (this.busy()) this.error.set('درخواست لغو شد.');
    this.busy.set(false);
  }
  @HostListener('document:keydown.escape')
  close(): void { this.cancel(); this.open.set(false); }
  ngOnDestroy(): void { this.cancel(); }
  formatBytes(value: number): string {
    if (value < 1024) return `${value} B`;
    const unit = Math.min(Math.floor(Math.log(value) / Math.log(1024)), 4);
    return `${(value / 1024 ** unit).toFixed(2)} ${['B', 'KB', 'MB', 'GB', 'TB'][unit]}`;
  }
  async copy(path: string): Promise<void> {
    try { await navigator.clipboard.writeText(path); }
    catch { this.error.set('کپی خودکار ممکن نیست؛ مسیر را انتخاب و کپی کنید.'); }
  }
  exportResult(): void {
    const result = this.result();
    if (!result) return;
    const url = URL.createObjectURL(new Blob([JSON.stringify(result, null, 2)], { type: 'application/json' }));
    const link = document.createElement('a'); link.href = url; link.download = 'duplicate-files.json'; link.click();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
  }
}
