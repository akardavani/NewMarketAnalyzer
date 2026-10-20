import { Component, inject, signal } from '@angular/core';
import { Subject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { ResourceFile, ResourceItem, ResourcesService } from './resources.service';

const PAGE = 20;

/** دکمهٔ هدر + مودال منابع (تگ، لینک، فایل‌های مستقل) */
@Component({
  selector: 'fd-resources-modal',
  templateUrl: './resources-modal.component.html',
  styleUrl: './resources-modal.component.scss'
})
export class ResourcesModalComponent {
  private readonly api = inject(ResourcesService);

  readonly open = signal(false);
  readonly items = signal<ResourceItem[]>([]);
  readonly loading = signal(false);
  readonly done = signal(false);
  readonly search = signal('');
  readonly error = signal('');
  readonly busy = signal(false);

  // فرم
  readonly activeId = signal(0);
  readonly title = signal('');
  readonly description = signal('');
  readonly link = signal('');
  readonly tags = signal<string[]>([]);
  readonly files = signal<ResourceFile[]>([]);
  readonly tagInput = signal('');
  readonly suggestions = signal<string[]>([]);

  private readonly tagTerm$ = new Subject<string>();
  private readonly search$ = new Subject<string>();

  constructor() {
    this.tagTerm$.pipe(debounceTime(250), distinctUntilChanged(),
      switchMap(t => this.api.tags(t))).subscribe({ next: s => this.suggestions.set(s.filter(x => !this.tags().includes(x))), error: () => this.suggestions.set([]) });
    this.search$.pipe(debounceTime(300), distinctUntilChanged()).subscribe(() => this.reload());
  }

  toggle(): void {
    this.open.update(v => !v);
    if (this.open()) { this.newItem(); this.reload(); }
  }
  close(): void { this.open.set(false); }

  onSearch(v: string): void { this.search.set(v); this.search$.next(v); }

  reload(): void { this.items.set([]); this.done.set(false); this.loadMore(); }

  loadMore(): void {
    if (this.loading() || this.done()) return;
    this.loading.set(true);
    this.api.getAll(this.search(), this.items().length, PAGE).subscribe({
      next: l => { this.items.update(x => [...x, ...l]); this.done.set(l.length < PAGE); this.loading.set(false); },
      error: e => { this.error.set(e.message); this.loading.set(false); }
    });
  }

  onScroll(e: Event): void {
    const el = e.target as HTMLElement;
    if (el.scrollTop + el.clientHeight >= el.scrollHeight - 40) this.loadMore();
  }

  newItem(): void { this.fill(null); }

  select(r: ResourceItem): void { this.fill(r); }

  private fill(r: ResourceItem | null): void {
    this.error.set('');
    this.activeId.set(r?.id ?? 0);
    this.title.set(r?.title ?? '');
    this.description.set(r?.description ?? '');
    this.link.set(r?.link ?? '');
    this.tags.set([...(r?.tags ?? [])]);
    this.files.set([...(r?.files ?? [])]);
    this.tagInput.set(''); this.suggestions.set([]);
  }

  // ---- تگ‌ها
  onTagInput(v: string): void {
    if (v.includes(';')) {
      const parts = v.split(';');
      parts.slice(0, -1).forEach(p => this.addTag(p));
      v = parts[parts.length - 1];
    }
    this.tagInput.set(v);
    const t = v.trim();
    if (t.length >= 3) this.tagTerm$.next(t); else this.suggestions.set([]);
  }
  addTag(raw: string): void {
    const t = raw.trim().toLowerCase().slice(0, 50);
    if (t && !this.tags().includes(t)) this.tags.update(x => [...x, t]);
    this.tagInput.set(''); this.suggestions.set([]);
  }
  onTagKey(e: KeyboardEvent): void {
    if (e.key === 'Enter') { e.preventDefault(); this.addTag(this.tagInput()); }
    else if (e.key === 'Backspace' && !this.tagInput() && this.tags().length) this.tags.update(x => x.slice(0, -1));
  }
  removeTag(t: string): void { this.tags.update(x => x.filter(y => y !== t)); }

  // ---- ذخیره
  save(): void {
    if (!this.title().trim()) { this.error.set('عنوان الزامی است.'); return; }
    if (this.tagInput().trim()) this.addTag(this.tagInput());
    this.busy.set(true); this.error.set('');
    this.api.save({ id: this.activeId(), title: this.title(), description: this.description(), link: this.link(), tags: this.tags() }).subscribe({
      next: id => { this.busy.set(false); this.activeId.set(id); this.reload(); },
      error: e => { this.busy.set(false); this.error.set(e.message); }
    });
  }

  remove(r: ResourceItem, ev: Event): void {
    ev.stopPropagation();
    if (!confirm('این مورد و فایل‌هایش حذف شود؟')) return;
    this.api.remove(r.id).subscribe({ next: () => { if (this.activeId() === r.id) this.newItem(); this.reload(); }, error: e => this.error.set(e.message) });
  }

  // ---- فایل‌ها (مستقل از ذخیرهٔ فرم)
  upload(input: HTMLInputElement, replace?: ResourceFile): void {
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;
    if (!this.activeId()) { this.error.set('ابتدا مورد را ذخیره کنید، سپس فایل اضافه کنید.'); return; }
    this.busy.set(true); this.error.set('');
    this.api.uploadFile(this.activeId(), replace?.id ?? 0, file).subscribe({
      next: f => {
        this.busy.set(false);
        this.files.update(l => replace ? l.map(x => x.id === replace.id ? f : x) : [...l, f]);
        this.syncFiles();
      },
      error: e => { this.busy.set(false); this.error.set(e.message || 'خطا در آپلود'); }
    });
  }
  removeFile(f: ResourceFile): void {
    this.api.removeFile(f.id).subscribe({
      next: () => { this.files.update(l => l.filter(x => x.id !== f.id)); this.syncFiles(); },
      error: e => this.error.set(e.message)
    });
  }
  download(f: ResourceFile): void {
    this.api.download(f).subscribe(b => {
      const a = document.createElement('a');
      a.href = URL.createObjectURL(b); a.download = f.fileName; a.click();
      URL.revokeObjectURL(a.href);
    });
  }
  /** فایل‌های آیتم در لیست را هم‌گام نگه می‌دارد */
  private syncFiles(): void {
    const id = this.activeId();
    this.items.update(l => l.map(x => x.id === id ? { ...x, files: this.files() } : x));
  }

  size(n: number): string {
    return n > 1048576 ? (n / 1048576).toFixed(1) + ' MB' : Math.max(1, Math.round(n / 1024)) + ' KB';
  }
  date(s: string): string { try { return new Date(s).toLocaleDateString('fa-IR'); } catch { return ''; } }
  safeLink(l: string): string { return /^https?:\/\//i.test(l) ? l : ''; }
}
