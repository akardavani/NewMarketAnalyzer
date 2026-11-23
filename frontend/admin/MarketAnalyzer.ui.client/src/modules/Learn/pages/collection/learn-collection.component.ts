import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RouterLink } from '@angular/router';
import { GlobalImports } from '@imports';
import { LearnApiService, CollectionContent, ImportPreview, LearningFile, LearningPlan } from '../../learn-api.service';

@Component({
  selector: 'learn-collection-page',
  imports: [...GlobalImports, RouterLink],
  templateUrl: './learn-collection.component.html',
  styleUrl: './learn-collection.component.scss'
})
export class LearnCollectionComponent implements OnInit {
  private readonly api = inject(LearnApiService);
  private readonly route = inject(ActivatedRoute);
  protected readonly content = signal<CollectionContent | null>(null);
  protected readonly plans = signal<LearningPlan[]>([]);
  protected readonly files = signal<LearningFile[]>([]);
  protected readonly preview = signal<ImportPreview | null>(null);
  protected readonly sessionId = signal<number | null>(null);
  protected readonly studiedIds = signal<Set<number>>(new Set());
  protected readonly selectedExportIds = signal<Set<number>>(new Set());
  protected readonly selectedBatchIds = signal<Set<number>>(new Set());
  protected readonly busy = signal(false);
  protected readonly message = signal('');
  protected readonly itemsPerBatch = signal(10);
  protected readonly batchDate = signal(this.localDate());
  protected readonly batchPlanId = signal<number | null>(null);
  private collectionId = 0;

  private localDate(): string {
    const now = new Date();
    return new Date(now.getTime() - now.getTimezoneOffset() * 60_000).toISOString().slice(0, 10);
  }

  ngOnInit(): void {
    this.collectionId = Number(this.route.snapshot.paramMap.get('id'));
    this.load();
  }

  protected load(): void {
    if (!this.collectionId) return;
    this.api.content(this.collectionId).subscribe({
      next: result => {
        const value = result.successed ? result.data ?? null : null;
        this.content.set(value);
        this.studiedIds.set(new Set(value?.items.filter(item => item.isStudied).map(item => item.id) ?? []));
        this.selectedExportIds.set(new Set(value?.items.map(item => item.id) ?? []));
      },
      error: () => this.message.set('دریافت محتوای مجموعه انجام نشد.')
    });
    this.api.plans(this.collectionId).subscribe({
      next: result => this.plans.set(result.successed ? result.data ?? [] : []),
      error: () => this.message.set('دریافت برنامه‌های مطالعه انجام نشد.')
    });
  }

  protected async filesChanged(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    this.preview.set(null);
    this.message.set('');
    try {
      const parsed = await Promise.all(files.map(async file => ({ fileName: file.name, content: await file.text() })));
      this.files.set(parsed);
      if (!parsed.length) return;
      this.busy.set(true);
      this.api.preview(this.collectionId, parsed).subscribe({
        next: result => { this.busy.set(false); this.preview.set(result.successed ? result.data ?? null : null); this.message.set(result.errors?.join(' ') ?? ''); },
        error: () => { this.busy.set(false); this.message.set('پیش‌نمایش فایل‌ها انجام نشد.'); }
      });
    } catch {
      this.message.set('خواندن یکی از فایل‌ها ممکن نشد.');
    }
    input.value = '';
  }

  protected confirmImport(): void {
    if (!this.files().length || !this.preview()?.items.length) return;
    this.busy.set(true);
    this.api.import(this.collectionId, this.files()).subscribe({
      next: result => {
        this.busy.set(false);
        this.message.set(result.successed ? 'محتوا وارد شد؛ موارد تکراری نادیده گرفته شدند.' : result.errors?.join(' ') ?? 'ورود محتوا ناموفق بود.');
        this.preview.set(null); this.files.set([]); this.load();
      },
      error: () => { this.busy.set(false); this.message.set('ثبت محتوا انجام نشد.'); }
    });
  }

  protected createPlan(): void {
    this.api.createPlan(this.collectionId, Math.max(1, Number(this.itemsPerBatch()) || 1)).subscribe({
      next: result => { this.message.set(result.successed ? 'برنامهٔ مطالعه و بسته‌های آن ساخته شد.' : result.errors?.join(' ') ?? 'ساخت برنامه ناموفق بود.'); this.load(); },
      error: () => this.message.set('ساخت برنامه انجام نشد.')
    });
  }

  protected setPlanStatus(plan: LearningPlan, status: number): void {
    this.api.setPlanStatus(plan.id, status).subscribe({
      next: result => { this.message.set(result.successed ? 'وضعیت برنامه به‌روز شد.' : result.errors?.join(' ') ?? 'تغییر وضعیت ناموفق بود.'); this.load(); },
      error: () => this.message.set('تغییر وضعیت برنامه انجام نشد.')
    });
  }

  protected addBatch(): void {
    const planId = this.batchPlanId();
    const ids = [...this.selectedBatchIds()];
    if (!planId) { this.message.set('یک برنامهٔ مطالعه انتخاب کنید.'); return; }
    if (!ids.length) { this.message.set('حداقل یک مورد برای بسته انتخاب کنید.'); return; }
    this.api.addBatch(planId, this.batchDate(), ids).subscribe({
      next: result => {
        this.message.set(result.successed ? 'بستهٔ مطالعه اضافه شد.' : result.errors?.join(' ') ?? 'افزودن بسته ناموفق بود.');
        if (result.successed) this.selectedBatchIds.set(new Set());
        this.load();
      },
      error: () => this.message.set('افزودن بستهٔ مطالعه انجام نشد.')
    });
  }

  protected startSession(): void {
    this.api.startSession(this.collectionId).subscribe({
      next: result => {
        if (result.successed && result.data) { this.sessionId.set(result.data); this.message.set('جلسهٔ مطالعه شروع شد.'); }
        else this.message.set(result.errors?.join(' ') ?? 'شروع جلسه ناموفق بود.');
      },
      error: () => this.message.set('شروع جلسه انجام نشد.')
    });
  }

  protected markStudied(itemId: number): void {
    const sessionId = this.sessionId();
    if (!sessionId) { this.message.set('ابتدا جلسهٔ مطالعه را شروع کنید.'); return; }
    this.api.recordProgress(sessionId, itemId).subscribe({
      next: result => {
        if (!result.successed) { this.message.set(result.errors?.join(' ') ?? 'ثبت پیشرفت ناموفق بود.'); return; }
        this.studiedIds.update(current => new Set(current).add(itemId));
      },
      error: () => this.message.set('ثبت پیشرفت انجام نشد.')
    });
  }

  protected review(itemId: number, rating: number): void {
    this.api.review(this.collectionId, itemId, rating).subscribe({
      next: result => this.message.set(result.successed ? 'ارزیابی مرور ثبت شد.' : result.errors?.join(' ') ?? 'ثبت مرور ناموفق بود.'),
      error: () => this.message.set('ثبت مرور انجام نشد.')
    });
  }

  protected exportSelected(): void {
    const ids = [...this.selectedExportIds()];
    if (!ids.length) { this.message.set('موردی برای خروجی وجود ندارد.'); return; }
    this.api.export(this.collectionId, ids).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a'); link.href = url; link.download = `learn-${this.collectionId}.html`; link.click(); URL.revokeObjectURL(url);
      },
      error: () => this.message.set('ساخت خروجی انجام نشد.')
    });
  }

  protected toggleExport(itemId: number, checked: boolean): void {
    this.selectedExportIds.update(current => {
      const next = new Set(current);
      if (checked) next.add(itemId); else next.delete(itemId);
      return next;
    });
  }

  protected toggleBatchItem(itemId: number, checked: boolean): void {
    this.selectedBatchIds.update(current => {
      const next = new Set(current);
      if (checked) next.add(itemId); else next.delete(itemId);
      return next;
    });
  }
}
