import { Component, computed, inject, signal } from '@angular/core';
import { PublishFolderItem, PublishService, PublishTargetItem } from './publish.service';

/** دکمهٔ هدر + مودال Publish (لیست مقصدها، ویرایش، اجرا) */
@Component({
  selector: 'fd-publish-modal',
  templateUrl: './publish-modal.component.html',
  styleUrl: './publish-modal.component.scss'
})
export class PublishModalComponent {
  private readonly api = inject(PublishService);

  readonly open = signal(false);
  readonly all = signal<PublishTargetItem[]>([]);
  readonly search = signal('');
  readonly error = signal('');
  readonly busy = signal(false);
  readonly checked = signal<string[]>([]);
  readonly resultPath = signal('');

  readonly items = computed(() => {
    const q = this.search().trim().toLowerCase();
    return q ? this.all().filter(x => (x.title + ' ' + x.key).toLowerCase().includes(q)) : this.all();
  });

  readonly activeId = signal(0);
  readonly key = signal('');
  readonly title = signal('');
  readonly folders = signal<PublishFolderItem[]>([]);
  readonly newName = signal('');
  readonly newPath = signal('');
  readonly outputPath = signal('');
  readonly displayOrder = signal(0);
  readonly isActive = signal(true);

  toggle(): void {
    this.open.update(v => !v);
    if (this.open()) { this.newItem(); this.reload(); }
  }
  close(): void { this.open.set(false); }

  reload(): void {
    this.api.getAll().subscribe({
      next: l => this.all.set(l),
      error: e => this.error.set(e.message)
    });
  }

  newItem(): void { this.fill(null); }
  select(r: PublishTargetItem): void { this.fill(r); }

  private fill(r: PublishTargetItem | null): void {
    this.error.set('');
    this.activeId.set(r?.id ?? 0);
    this.key.set(r?.key ?? '');
    this.title.set(r?.title ?? '');
    this.folders.set((r?.folders ?? []).map(f => ({ ...f })));
    this.newName.set(''); this.newPath.set('');
    this.outputPath.set(r?.outputPath ?? '');
    this.displayOrder.set(r?.displayOrder ?? 0);
    this.isActive.set(r?.isActive ?? true);
  }

  save(): void {
    if (!this.key().trim() || !this.title().trim() || !this.outputPath().trim()) {
      this.error.set('کلید، عنوان و مسیر خروجی الزامی است.'); return;
    }
    if (this.newName().trim() || this.newPath().trim()) { this.addFolder(); if (this.error()) return; }
    if (!this.folders().length) { this.error.set('حداقل یک پوشه‌ی پروژه لازم است.'); return; }
    this.busy.set(true); this.error.set('');
    this.api.save({
      id: this.activeId(), key: this.key(), title: this.title(), folders: this.folders(), outputPath: this.outputPath(), displayOrder: this.displayOrder(), isActive: this.isActive()
    }).subscribe({
      next: id => { this.busy.set(false); this.activeId.set(id); this.reload(); },
      error: e => { this.busy.set(false); this.error.set(e.message); }
    });
  }

  remove(r: PublishTargetItem, ev: Event): void {
    ev.stopPropagation();
    if (!confirm(`«${r.title}» حذف شود؟`)) return;
    this.api.remove(r.id).subscribe({
      next: () => { if (this.activeId() === r.id) this.newItem(); this.checked.update(l => l.filter(k => k !== r.key)); this.reload(); },
      error: e => this.error.set(e.message)
    });
  }

  addFolder(): void {
    const name = this.newName().trim(), path = this.newPath().trim();
    if (!name && !path) return;
    if (!name || !path) { this.error.set('نام و مسیر پوشه هر دو لازم است.'); return; }
    if (this.folders().some(f => f.name.toLowerCase() === name.toLowerCase())) { this.error.set('نام پوشه تکراری است.'); return; }
    this.error.set('');
    this.folders.update(l => [...l, { name, path }]);
    this.newName.set(''); this.newPath.set('');
  }
  removeFolder(f: PublishFolderItem): void { this.folders.update(l => l.filter(x => x !== f)); }

  folderNames(r: PublishTargetItem): string { return r.folders.map(f => f.name).join('، '); }
  isChecked(k: string): boolean { return this.checked().includes(k); }
  check(r: PublishTargetItem, ev: Event): void {
    ev.stopPropagation();
    this.checked.update(l => l.includes(r.key) ? l.filter(k => k !== r.key) : [...l, r.key]);
  }

  run(): void {
    if (!this.checked().length) { this.error.set('حداقل یک مورد را تیک بزنید.'); return; }
    this.busy.set(true); this.error.set(''); this.resultPath.set('');
    this.api.run(this.checked()).subscribe({
      next: r => { this.busy.set(false); this.resultPath.set(r.path); },
      error: e => { this.busy.set(false); this.error.set(e.message); }
    });
  }
  copy(): void { navigator.clipboard.writeText(this.resultPath()); }
}
