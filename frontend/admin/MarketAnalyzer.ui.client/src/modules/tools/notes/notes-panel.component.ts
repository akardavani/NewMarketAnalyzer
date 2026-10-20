import { Component, computed, ElementRef, HostListener, inject, signal } from '@angular/core';
import { Subject, debounceTime, switchMap } from 'rxjs';
import { Note, NotesService } from './notes.service';

type SaveState = 'idle' | 'saving' | 'saved' | 'error';

/** دکمهٔ هدر + پنل یادداشت‌ها (چند یادداشت، ذخیرهٔ خودکار) */
@Component({
  selector: 'fd-notes-panel',
  templateUrl: './notes-panel.component.html',
  styleUrl: './notes-panel.component.scss'
})
export class NotesPanelComponent {
  private readonly api = inject(NotesService);
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  readonly open = signal(false);
  readonly notes = signal<Note[]>([]);
  readonly activeId = signal<number | null>(null);
  readonly state = signal<SaveState>('idle');
  readonly sortByColor = signal(false);

  readonly sorted = computed(() => {
    const l = this.notes();
    return this.sortByColor() ? [...l].sort((a, b) => this.hueKey(a.color) - this.hueKey(b.color)) : l;
  });

  private readonly changes$ = new Subject<Note>();

  constructor() {
    this.changes$.pipe(
      debounceTime(1000),
      switchMap(n => { this.state.set('saving'); return this.api.update(n.id, n.title, n.content, n.color); })
    ).subscribe({
      next: () => this.state.set('saved'),
      error: () => this.state.set('error')
    });
  }

  get active(): Note | undefined { return this.notes().find(n => n.id === this.activeId()); }

  private seedState?: { id: number; html: string };

  /** محتوای اولیهٔ ادیتور؛ ثابت می‌ماند تا با هر تایپ innerHTML دوباره ست نشود و کرسر به اول نپرد */
  seed(n: Note): string {
    if (this.seedState?.id !== n.id) this.seedState = { id: n.id, html: n.content };
    return this.seedState.html;
  }

  toggle(): void {
    this.seedState = undefined;
    this.open.update(v => !v);
    if (this.open()) this.load();
  }

  @HostListener('document:mousedown', ['$event'])
  onDocClick(e: MouseEvent): void {
    if (this.open() && !this.host.nativeElement.contains(e.target as Node)) this.close();
  }

  @HostListener('document:keydown.escape')
  close(): void { this.flush(); this.open.set(false); }

  private load(): void {
    this.api.getAll().subscribe({
      next: list => {
        this.notes.set(list);
        if (!list.some(n => n.id === this.activeId())) this.activeId.set(list[0]?.id ?? null);
      },
      error: () => this.state.set('error')
    });
  }

  select(id: number): void { this.flush(); this.seedState = undefined; this.activeId.set(id); this.state.set('idle'); }

  add(): void {
    this.api.create('', '').subscribe(id => {
      const n: Note = { id, title: '', content: '', lastModificationTime: new Date().toISOString() };
      this.notes.update(l => [n, ...l]);
      this.activeId.set(id);
    });
  }

  remove(n: Note): void {
    if (!confirm('این یادداشت حذف شود؟')) return;
    this.api.remove(n.id).subscribe(() => {
      this.notes.update(l => l.filter(x => x.id !== n.id));
      if (this.activeId() === n.id) this.activeId.set(this.notes()[0]?.id ?? null);
    });
  }

  onTitle(n: Note, value: string): void { n.title = value; this.changed(n); }

  onContent(n: Note, el: HTMLElement): void {
    n.content = el.innerHTML;
    this.changed(n);
  }

  /** بعد از هر تغییر ذخیره‌ی خودکار با debounce */
  private changed(n: Note): void { this.state.set('idle'); this.pending = n; this.changes$.next({ ...n }); }
  private pending?: Note;

  /** هنگام بستن/تعویض، تغییر ذخیره‌نشده فوراً ارسال می‌شود */
  private flush(): void {
    const n = this.pending;
    if (!n) return;
    this.pending = undefined;
    this.api.update(n.id, n.title, n.content, n.color).subscribe({ next: () => this.state.set('saved'), error: () => this.state.set('error') });
  }

  // ---- toolbar
  exec(cmd: string, value?: string): void { document.execCommand(cmd, false, value); this.syncFromDom(); }

  heading(tag: string): void { this.exec('formatBlock', tag); }

  link(): void {
    const url = prompt('آدرس لینک (https://...)');
    if (url) this.exec('createLink', url);
  }

  checklist(): void {
    document.execCommand('insertUnorderedList');
    const sel = window.getSelection();
    const ul = (sel?.anchorNode as Node | null)?.parentElement?.closest('ul');
    if (ul) ul.getAttribute('data-type') === 'todo' ? ul.removeAttribute('data-type') : ul.setAttribute('data-type', 'todo');
    this.syncFromDom();
  }

  /** کلیک روی چک‌باکس (سمت راست آیتم) وضعیت را تغییر می‌دهد */
  onEditorClick(ev: MouseEvent): void {
    const li = ev.target as HTMLElement;
    if (li.tagName !== 'LI' || li.parentElement?.getAttribute('data-type') !== 'todo') return;
    const rect = li.getBoundingClientRect();
    if (rect.right - ev.clientX > 24) return;
    li.getAttribute('data-checked') === 'true' ? li.removeAttribute('data-checked') : li.setAttribute('data-checked', 'true');
    this.syncFromDom();
  }

  private syncFromDom(): void {
    const el = this.host.nativeElement.querySelector<HTMLElement>('.editor');
    const n = this.active;
    if (el && n) this.onContent(n, el);
  }

  /** کلید مرتب‌سازی رنگ: بی‌رنگ‌ها آخر، بقیه بر اساس hue و سپس روشنایی */
  private hueKey(hex?: string | null): number {
    if (!hex || !/^#[0-9a-f]{6}$/i.test(hex)) return 1e9;
    const r = parseInt(hex.slice(1, 3), 16) / 255, g = parseInt(hex.slice(3, 5), 16) / 255, b = parseInt(hex.slice(5, 7), 16) / 255;
    const max = Math.max(r, g, b), min = Math.min(r, g, b), d = max - min;
    if (d === 0) return 1000 + max;
    const h = max === r ? ((g - b) / d + 6) % 6 : max === g ? (b - r) / d + 2 : (r - g) / d + 4;
    return h * 60 + max / 10;
  }

  onColor(n: Note, value: string): void { n.color = value; this.notes.update(l => [...l]); this.changed(n); }
  clearColor(n: Note): void { n.color = null; this.notes.update(l => [...l]); this.changed(n); }

  trackId = (_: number, n: Note) => n.id;
  label(n: Note): string { return n.title.trim() || 'بدون عنوان'; }
}
