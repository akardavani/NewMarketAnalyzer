import { ChangeDetectionStrategy, Component, HostListener, OnDestroy, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ShufflePlan, ShuffleRequest, ShuffleResult, ShuffleVideoService } from './shuffle-video.service';

@Component({
  selector: 'fd-shuffle-video-modal',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './shuffle-video-modal.component.html',
  styleUrl: './shuffle-video-modal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ShuffleVideoModalComponent implements OnDestroy {
  private readonly api = inject(ShuffleVideoService);
  private request?: Subscription;
  readonly open = signal(false);
  readonly busy = signal<'preview' | 'copy' | null>(null);
  readonly error = signal('');
  readonly plan = signal<ShufflePlan | null>(null);
  readonly result = signal<ShuffleResult | null>(null);
  readonly incomplete = computed(() => this.plan()?.folders.some(f => !f.complete) ?? false);
  readonly fileCount = computed(() => this.plan()?.folders.reduce((n, f) => n + f.files.length, 0) ?? 0);
  allowIncomplete = false;
  form: ShuffleRequest = { sourcePath: '', outputPath: '', prefix: 'Mix-', folderCount: 1, targetMinutes: 60 };
  show(): void { this.open.set(true); }
  close(): void { this.cancel(); this.open.set(false); }
  @HostListener('document:keydown.escape')
  onEscape(): void { if (this.open()) this.close(); }
  invalidate(): void { this.plan.set(null); this.result.set(null); this.allowIncomplete = false; }
  preview(): void {
    if (this.busy()) return;
    this.invalidate(); this.error.set('');
    if (!this.form.sourcePath.trim() || !this.form.outputPath.trim() ||
        !Number.isInteger(this.form.folderCount) || this.form.folderCount < 1 || this.form.folderCount > 50 ||
        !Number.isInteger(this.form.targetMinutes) || this.form.targetMinutes < 1 || this.form.targetMinutes > 1440) {
      this.error.set('مسیرها، تعداد پوشه (۱ تا ۵۰) و مدت به دقیقه (۱ تا ۱۴۴۰) را درست وارد کنید.'); return;
    }
    this.busy.set('preview');
    this.request = this.api.preview({ ...this.form, sourcePath: this.form.sourcePath.trim(), outputPath: this.form.outputPath.trim() }).subscribe({
      next: p => { this.plan.set(p); this.busy.set(null); },
      error: e => this.failed(e)
    });
  }
  run(): void {
    const p = this.plan();
    if (!p || this.busy() || !this.fileCount() || (this.incomplete() && !this.allowIncomplete)) return;
    if (Date.parse(p.expiresUtc) <= Date.now()) { this.invalidate(); this.error.set('پیش‌نمایش منقضی شده است.'); return; }
    if (!confirm('فایل‌های انتخاب‌شده در پوشهٔ جدید مقصد کپی و در سابقه ثبت شوند؟')) return;
    this.busy.set('copy'); this.error.set('');
    this.request = this.api.run(p.id, this.allowIncomplete).subscribe({
      next: r => { this.result.set(r); this.plan.set(null); this.busy.set(null); },
      error: e => { this.plan.set(null); this.failed(e); }
    });
  }
  private failed(e: { status?: number; error?: { message?: string }; message?: string }): void {
    this.busy.set(null);
    this.error.set(e.status === 404 ? 'ابزار در تنظیمات سرور فعال نشده است.' :
      e.status === 403 ? 'شناسهٔ کاربر شما در فهرست مجاز سرور نیست یا مجوز این عملیات را ندارید.' :
      e.error?.message || e.message || 'خطا در ارتباط با سرور؛ در صورت شروع کپی، مقصد را نیز بررسی کنید.');
  }
  cancel(): void {
    this.request?.unsubscribe(); this.request = undefined;
    if (this.busy()) {
      const copying = this.busy() === 'copy';
      this.invalidate(); this.busy.set(null);
      this.error.set(copying ? 'درخواست لغو ارسال شد. اگر ثبت نهایی آغاز شده باشد، خروجی کامل می‌شود؛ مقصد را بررسی کنید.' : 'پیش‌نمایش لغو شد.');
    }
  }
  ngOnDestroy(): void { this.cancel(); }
  minutes(seconds: number): string { return (seconds / 60).toFixed(2); }
  bytes(value: number): string { return (value / 1024 ** 3).toFixed(3) + ' GiB'; }
  exportPlan(): void {
    const plan = this.plan(); if (!plan) return;
    const url = URL.createObjectURL(new Blob([JSON.stringify(plan, null, 2)], { type: 'application/json' }));
    const link = document.createElement('a'); link.href = url; link.download = 'shuffle-preview.json'; link.click();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
  }
}
