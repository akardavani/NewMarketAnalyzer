import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DateInputComponent } from '@components/date-input/date-input.component';
import { Subject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { BookmarkItem, BookmarkKind, BookmarksService, CollectionItem, KIND_LABELS, TagItem } from './bookmarks.service';

const PAGE = 24;
interface Node { c: CollectionItem; depth: number; }

/** دکمهٔ هدر + مودال مدیریت لینک‌ها (مجموعهٔ تودرتو، تگ، علاقه‌مندی، نمای لیست/کارت) */
@Component({
  selector: 'fd-bookmarks-modal',
  imports: [FormsModule, DateInputComponent],
  templateUrl: './bookmarks-modal.component.html',
  styleUrl: './bookmarks-modal.component.scss'
})
export class BookmarksModalComponent {
  private readonly api = inject(BookmarksService);
  readonly kinds = Object.entries(KIND_LABELS).map(([k, v]) => ({ k: +k, v }));
  readonly kindLabel = (k: number) => KIND_LABELS[k] ?? '';

  readonly open = signal(false);
  readonly view = signal<'list' | 'card'>('list');
  readonly items = signal<BookmarkItem[]>([]);
  readonly collections = signal<CollectionItem[]>([]);
  readonly allTags = signal<TagItem[]>([]);
  readonly loading = signal(false);
  readonly done = signal(false);
  readonly error = signal('');
  readonly busy = signal(false);

  // فیلترها
  readonly search = signal('');
  readonly fTag = signal('');
  readonly fCollection = signal<number | null>(null);
  readonly fFav = signal(false);
  readonly fKind = signal<number | null>(null);
  readonly fFrom = signal<Date | null>(null);
  readonly fTo = signal<Date | null>(null);

  // فرم
  readonly activeId = signal(0);
  readonly url = signal('');
  readonly title = signal('');
  readonly note = signal('');
  readonly imageUrl = signal<string | null>(null);
  readonly kind = signal<BookmarkKind>(0);
  readonly collectionId = signal<number | null>(null);
  readonly tags = signal<string[]>([]);
  readonly tagInput = signal('');
  readonly suggestions = signal<string[]>([]);
  readonly fetching = signal(false);
  readonly showForm = signal(false);

  readonly tree = computed<Node[]>(() => {
    const all = this.collections(); const out: Node[] = [];
    const walk = (pid: number | null, d: number, seen: Set<number>) => {
      for (const c of all.filter(x => (x.parentId ?? null) === pid)) {
        if (seen.has(c.id)) continue;
        out.push({ c, depth: d }); walk(c.id, d + 1, new Set(seen).add(c.id));
      }
    };
    walk(null, 0, new Set());
    return out;
  });

  private readonly tagTerm$ = new Subject<string>();
  private readonly search$ = new Subject<string>();

  constructor() {
    this.tagTerm$.pipe(debounceTime(250), distinctUntilChanged(), switchMap(t => this.api.tags(t)))
      .subscribe({ next: s => this.suggestions.set(s.map(x => x.name).filter(x => !this.tags().includes(x))), error: () => this.suggestions.set([]) });
    this.search$.pipe(debounceTime(300), distinctUntilChanged()).subscribe(() => this.reload());
  }

  toggle(): void {
    this.open.update(v => !v);
    if (this.open()) { this.newItem(); this.showForm.set(false); this.loadSide(); this.reload(); }
  }
  close(): void { this.open.set(false); }

  loadSide(): void {
    this.api.collections().subscribe({ next: c => this.collections.set(c), error: e => this.error.set(e.message) });
    this.api.tags('').subscribe({ next: t => this.allTags.set(t), error: () => { } });
  }

  onSearch(v: string): void { this.search.set(v); this.search$.next(v); }

  reload(): void { this.items.set([]); this.done.set(false); this.loadMore(); }

  loadMore(): void {
    if (this.loading() || this.done()) return;
    this.loading.set(true);
    this.api.getAll({
      search: this.search(), tag: this.fTag(), collectionId: this.fCollection(), favoritesOnly: this.fFav(),
      kind: this.fKind(), from: this.iso(this.fFrom()), to: this.iso(this.fTo()), skip: this.items().length, take: PAGE
    }).subscribe({
      next: l => { this.items.update(x => [...x, ...l]); this.done.set(l.length < PAGE); this.loading.set(false); },
      error: e => { this.error.set(e.message); this.loading.set(false); }
    });
  }
  private iso(d: Date | null): string | undefined {
    if (!d) return undefined;
    const p = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}`;
  }
  onScroll(e: Event): void {
    const el = e.target as HTMLElement;
    if (el.scrollTop + el.clientHeight >= el.scrollHeight - 40) this.loadMore();
  }

  // ---- فیلتر
  setCollection(id: number | null): void { this.fCollection.set(id); this.fFav.set(false); this.reload(); }
  setFav(): void { this.fFav.set(true); this.fCollection.set(null); this.reload(); }
  setTag(t: string): void { this.fTag.set(this.fTag() === t ? '' : t); this.reload(); }
  setKind(v: string): void { this.fKind.set(v === '' ? null : +v); this.reload(); }
  setDate(which: 'from' | 'to', v: Date | null): void { (which === 'from' ? this.fFrom : this.fTo).set(v); this.reload(); }
  clearFilters(): void {
    this.search.set(''); this.fTag.set(''); this.fCollection.set(null); this.fFav.set(false);
    this.fKind.set(null); this.fFrom.set(null); this.fTo.set(null); this.reload();
  }
  get hasFilter(): boolean {
    return !!(this.search() || this.fTag() || this.fCollection() || this.fFav() || this.fKind() !== null || this.fFrom() || this.fTo());
  }

  // ---- مجموعه‌ها
  addCollection(parent: CollectionItem | null): void {
    const name = prompt(parent ? `نام زیرمجموعه در «${parent.name}»` : 'نام مجموعه')?.trim();
    if (!name) return;
    this.api.saveCollection(0, name, parent?.id ?? null).subscribe({ next: () => this.loadSide(), error: e => this.error.set(e.message) });
  }
  renameCollection(c: CollectionItem, ev: Event): void {
    ev.stopPropagation();
    const name = prompt('نام جدید', c.name)?.trim();
    if (!name || name === c.name) return;
    this.api.saveCollection(c.id, name, c.parentId ?? null).subscribe({ next: () => this.loadSide(), error: e => this.error.set(e.message) });
  }
  removeCollection(c: CollectionItem, ev: Event): void {
    ev.stopPropagation();
    if (!confirm(`مجموعهٔ «${c.name}» حذف شود؟ لینک‌های آن حذف نمی‌شوند.`)) return;
    this.api.removeCollection(c.id).subscribe({
      next: () => { if (this.fCollection() === c.id) this.fCollection.set(null); this.loadSide(); this.reload(); },
      error: e => this.error.set(e.message)
    });
  }

  // ---- تگ (مدیریت)
  renameTag(t: TagItem, ev: Event): void {
    ev.stopPropagation();
    const n = prompt('نام جدید تگ', t.name)?.trim().toLowerCase();
    if (!n || n === t.name) return;
    this.api.renameTag(t.name, n).subscribe({
      next: () => { if (this.fTag() === t.name) this.fTag.set(n); this.loadSide(); this.reload(); }, error: e => this.error.set(e.message)
    });
  }
  removeTagGlobal(t: TagItem, ev: Event): void {
    ev.stopPropagation();
    if (!confirm(`تگ «${t.name}» از همهٔ لینک‌ها حذف شود؟`)) return;
    this.api.removeTag(t.name).subscribe({
      next: () => { if (this.fTag() === t.name) this.fTag.set(''); this.loadSide(); this.reload(); }, error: e => this.error.set(e.message)
    });
  }

  // ---- فرم
  newItem(): void { this.fill(null); this.showForm.set(true); }
  select(b: BookmarkItem): void { this.fill(b); this.showForm.set(true); }
  private fill(b: BookmarkItem | null): void {
    this.error.set('');
    this.activeId.set(b?.id ?? 0);
    this.url.set(b?.url ?? ''); this.title.set(b?.title ?? ''); this.note.set(b?.note ?? '');
    this.imageUrl.set(b?.imageUrl ?? null); this.kind.set(b?.kind ?? 0);
    this.collectionId.set(b?.collectionId ?? this.fCollection());
    this.tags.set([...(b?.tags ?? [])]); this.tagInput.set(''); this.suggestions.set([]);
  }

  /** دریافت خودکار عنوان/تصویر از آدرس */
  autoFill(): void {
    const u = this.url().trim();
    if (!/^https?:\/\//i.test(u)) { this.error.set('آدرس باید با http:// یا https:// شروع شود.'); return; }
    this.fetching.set(true); this.error.set('');
    this.api.preview(u).subscribe({
      next: m => {
        this.fetching.set(false);
        if (!this.title().trim()) this.title.set(m.title);
        this.imageUrl.set(m.imageUrl ?? null); this.kind.set(m.kind);
      },
      error: e => { this.fetching.set(false); this.error.set(e.message || 'دریافت اطلاعات ممکن نشد'); }
    });
  }

  setFormKind(v: string): void { this.kind.set(+v as BookmarkKind); }

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

  save(): void {
    if (!/^https?:\/\//i.test(this.url().trim())) { this.error.set('آدرس معتبر (http/https) الزامی است.'); return; }
    if (this.tagInput().trim()) this.addTag(this.tagInput());
    this.busy.set(true); this.error.set('');
    this.api.save({
      id: this.activeId(), url: this.url().trim(), title: this.title(), note: this.note(), imageUrl: this.imageUrl(),
      kind: this.kind(), collectionId: this.collectionId(), tags: this.tags()
    }).subscribe({
      next: id => { this.busy.set(false); this.activeId.set(id); this.loadSide(); this.reload(); },
      error: e => { this.busy.set(false); this.error.set(e.message); }
    });
  }

  remove(b: BookmarkItem, ev: Event): void {
    ev.stopPropagation();
    if (!confirm('این لینک حذف شود؟')) return;
    this.api.remove(b.id).subscribe({
      next: () => { if (this.activeId() === b.id) this.newItem(); this.loadSide(); this.reload(); }, error: e => this.error.set(e.message)
    });
  }

  toggleFav(b: BookmarkItem, ev: Event): void {
    ev.stopPropagation();
    const v = !b.isFavorite;
    this.api.setFavorite(b.id, v).subscribe({
      next: () => this.items.update(l => this.fFav() && !v ? l.filter(x => x.id !== b.id) : l.map(x => x.id === b.id ? { ...x, isFavorite: v } : x)),
      error: e => this.error.set(e.message)
    });
  }

  date(s: string): string { try { return new Date(s).toLocaleDateString('fa-IR'); } catch { return ''; } }
  safeLink(l: string): string { return /^https?:\/\//i.test(l) ? l : ''; }
  host(l: string): string { try { return new URL(l).host; } catch { return l; } }
}
