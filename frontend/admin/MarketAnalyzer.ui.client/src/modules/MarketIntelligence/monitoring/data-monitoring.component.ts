import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { DataJobRun, DataJobRunDetail, DataJobRunIssue, DataMonitoringService } from './data-monitoring.service';

@Component({
  selector: 'mi-data-monitoring',
  imports: [CommonModule],
  templateUrl: './data-monitoring.component.html',
  styleUrl: './data-monitoring.component.scss'
})
export class DataMonitoringComponent implements OnInit {
  private readonly service = inject(DataMonitoringService);
  readonly runs = signal<DataJobRun[]>([]);
  readonly details = signal<Record<string, DataJobRunDetail>>({});
  readonly selectedIssues = signal<Record<string, Set<string>>>({});
  readonly expandedRunId = signal<string | null>(null);
  readonly loading = signal(false);
  readonly busyRunId = signal<string | null>(null);
  readonly error = signal('');
  readonly notice = signal('');

  ngOnInit(): void { this.reload(); }

  reload(): void {
    this.loading.set(true);
    this.error.set('');
    this.service.recentRuns().subscribe({
      next: rows => { this.runs.set(rows); this.loading.set(false); },
      error: e => { this.error.set(this.errorMessage(e)); this.loading.set(false); }
    });
  }

  toggleDetails(run: DataJobRun): void {
    if (this.expandedRunId() === run.id) { this.expandedRunId.set(null); return; }
    this.expandedRunId.set(run.id);
    if (this.details()[run.id]) return;
    this.busyRunId.set(run.id);
    this.service.runDetails(run.id).subscribe({
      next: value => { this.details.update(all => ({ ...all, [run.id]: value })); this.busyRunId.set(null); },
      error: e => { this.error.set(this.errorMessage(e)); this.busyRunId.set(null); }
    });
  }

  issueSelected(runId: string, issueId: string, checked: boolean): void {
    this.selectedIssues.update(all => {
      const next = new Set(all[runId] ?? []);
      checked ? next.add(issueId) : next.delete(issueId);
      return { ...all, [runId]: next };
    });
  }

  selectedCount(runId: string): number { return this.selectedIssues()[runId]?.size ?? 0; }

  deleteSelected(runId: string): void {
    const ids = [...(this.selectedIssues()[runId] ?? [])];
    if (!ids.length || !window.confirm(`جزئیات ${ids.length} خطای انتخاب‌شده حذف شود؟ خلاصهٔ اجرای کار باقی می‌ماند.`)) return;
    this.busyRunId.set(runId);
    this.service.deleteIssues(runId, ids).subscribe({
      next: result => {
        this.selectedIssues.update(all => ({ ...all, [runId]: new Set<string>() }));
        this.notice.set(`${result.deletedCount} ردیف خطا حذف شد؛ خلاصهٔ اجرا نگه داشته شد.`);
        this.service.runDetails(runId).subscribe({
          next: value => { this.details.update(all => ({ ...all, [runId]: value })); this.busyRunId.set(null); this.reload(); },
          error: e => { this.error.set(this.errorMessage(e)); this.busyRunId.set(null); }
        });
      },
      error: e => { this.error.set(this.errorMessage(e)); this.busyRunId.set(null); }
    });
  }

  statusLabel(status: string): string {
    return ({ Running: 'در حال اجرا', Completed: 'موفق', Partial: 'موفق با خطا', Failed: 'ناموفق', Skipped: 'ردشده', Cancelled: 'لغوشده' } as Record<string, string>)[status] ?? status;
  }

  date(value: string | null | undefined, timezone?: string | null): string {
    if (!value) return '—';
    try {
      return new Intl.DateTimeFormat('fa-IR-u-ca-persian-nu-latn', {
        timeZone: timezone || 'Asia/Tehran', year: 'numeric', month: '2-digit', day: '2-digit',
        hour: '2-digit', minute: '2-digit', hourCycle: 'h23'
      }).format(new Date(value));
    } catch { return new Date(value).toLocaleString('fa-IR'); }
  }

  trackIssue(_: number, issue: DataJobRunIssue): string { return issue.id; }
  private errorMessage(error: any): string { return error?.error?.errors?.join(' ') || error?.message || 'خطا در دریافت اطلاعات مانیتورینگ.'; }
}
