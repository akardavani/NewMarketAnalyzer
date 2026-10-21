import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PasswordsService } from './passwords.service';
import { VAULT_ITERATIONS, VaultCryptoService } from './vault-crypto.service';

/** داده‌ی یک ورودی؛ فقط به‌صورت رمزشده روی سرور است */
export interface Secret { title: string; username: string; password: string; url: string; category: string; notes: string; }
interface Row { id: number; data: Secret; modified: string; }

const IDLE_MS = 5 * 60_000;      // قفل خودکار بعد از ۵ دقیقه بی‌فعالیتی
const CLIP_MS = 20_000;          // پاک‌سازی کلیپ‌بورد بعد از ۲۰ ثانیه
const EMPTY: Secret = { title: '', username: '', password: '', url: '', category: '', notes: '' };

/** دکمهٔ هدر + مودال مدیریت رمز (کاسه‌ی رمزنگاری‌شده در مرورگر) */
@Component({
  selector: 'fd-passwords-modal',
  imports: [FormsModule],
  templateUrl: './passwords-modal.component.html',
  styleUrl: './passwords-modal.component.scss'
})
export class PasswordsModalComponent {
  private readonly api = inject(PasswordsService);
  readonly crypto = inject(VaultCryptoService);

  readonly open = signal(false);
  readonly state = signal<'loading' | 'setup' | 'locked' | 'unlocked'>('loading');
  readonly busy = signal(false);
  readonly error = signal('');
  readonly info = signal('');

  // ورود / راه‌اندازی
  readonly master = signal('');
  readonly master2 = signal('');
  readonly showChangeMaster = signal(false);
  readonly newMaster = signal('');
  readonly newMaster2 = signal('');

  // داده‌ها (فقط در حافظه و پس از قفل‌گشایی)
  readonly rows = signal<Row[]>([]);
  readonly search = signal('');
  readonly fCat = signal<string>('');
  readonly selectedId = signal<number | null>(null);
  readonly editing = signal(false);
  readonly draft = signal<Secret>({ ...EMPTY });
  readonly draftId = signal<number>(0);
  readonly reveal = signal(false);

  // مولد رمز
  readonly genLen = signal(20);
  readonly genUpper = signal(true);
  readonly genLower = signal(true);
  readonly genDigits = signal(true);
  readonly genSymbols = signal(true);

  readonly categories = computed(() => [...new Set(this.rows().map(r => r.data.category).filter(Boolean))].sort());
  readonly reusedIds = computed(() => {
    const count = new Map<string, number[]>();
    for (const r of this.rows()) { if (!r.data.password) continue; const a = count.get(r.data.password) ?? []; a.push(r.id); count.set(r.data.password, a); }
    const out = new Set<number>();
    for (const ids of count.values()) if (ids.length > 1) ids.forEach(i => out.add(i));
    return out;
  });
  readonly weakCount = computed(() => this.rows().filter(r => r.data.password && this.crypto.strength(r.data.password) <= 1).length);
  readonly filtered = computed(() => {
    const q = this.search().trim().toLowerCase();
    return this.rows().filter(r => (!this.fCat() || r.data.category === this.fCat()) &&
      (!q || [r.data.title, r.data.username, r.data.url, r.data.category].some(x => x.toLowerCase().includes(q))))
      .sort((a, b) => a.data.title.localeCompare(b.data.title));
  });
  readonly selected = computed(() => this.rows().find(r => r.id === this.selectedId()) ?? null);
  readonly draftStrength = computed(() => this.crypto.strength(this.draft().password));

  private key: CryptoKey | null = null;
  private status: { salt: string; iterations: number; verifierCipher: string; verifierIv: string } | null = null;
  private lastActivity = Date.now();
  private idleTimer: ReturnType<typeof setInterval> | null = null;
  private clipTimer: ReturnType<typeof setTimeout> | null = null;

  toggle(): void { this.open.update(v => !v); if (this.open()) this.init(); else this.lock(); }
  close(): void { this.open.set(false); this.lock(); }

  private init(): void {
    this.error.set(''); this.info.set(''); this.state.set('loading');
    this.api.status().subscribe({
      next: s => {
        if (!s.initialized) { this.state.set('setup'); return; }
        this.status = { salt: s.salt!, iterations: s.iterations, verifierCipher: s.verifierCipher!, verifierIv: s.verifierIv! };
        this.state.set('locked');
      },
      error: e => { this.error.set(e.message); this.state.set('locked'); }
    });
  }

  /** راه‌اندازی اولیه‌ی کاسه با master password جدید */
  setup(): void {
    this.error.set('');
    if (this.master().length < 10) return this.error.set('master password حداقل ۱۰ کاراکتر باشد.');
    if (this.master() !== this.master2()) return this.error.set('تکرار master password یکسان نیست.');
    this.busy.set(true);
    const salt = this.crypto.newSalt();
    this.crypto.deriveKey(this.master(), salt, VAULT_ITERATIONS).then(k => this.crypto.makeVerifier(k).then(v => ({ k, v })))
      .then(({ k, v }) => this.api.init({ salt, iterations: VAULT_ITERATIONS, verifierCipher: v.cipher, verifierIv: v.iv }).subscribe({
        next: () => {
          this.status = { salt, iterations: VAULT_ITERATIONS, verifierCipher: v.cipher, verifierIv: v.iv };
          this.key = k; this.clearMaster(); this.afterUnlock();
        },
        error: e => this.fail(e.message)
      }))
      .catch(e => this.fail(String(e?.message ?? e)));
  }

  /** قفل‌گشایی: کلید از master و salt ساخته می‌شود و با verifier بررسی می‌شود */
  unlock(): void {
    if (!this.status) return;
    this.error.set(''); this.busy.set(true);
    this.crypto.deriveKey(this.master(), this.status.salt, this.status.iterations)
      .then(async k => {
        const ok = await this.crypto.checkVerifier(k, { cipher: this.status!.verifierCipher, iv: this.status!.verifierIv });
        if (!ok) throw new Error('master password اشتباه است.');
        this.key = k; this.clearMaster(); this.afterUnlock();
      })
      .catch(e => this.fail(e.message));
  }

  lock(): void {
    this.key = null; this.rows.set([]); this.selectedId.set(null); this.editing.set(false); this.reveal.set(false);
    this.draft.set({ ...EMPTY }); this.clearMaster(); this.showChangeMaster.set(false);
    if (this.idleTimer) { clearInterval(this.idleTimer); this.idleTimer = null; }
    if (this.state() === 'unlocked') this.state.set('locked');
  }

  private afterUnlock(): void {
    this.busy.set(false); this.state.set('unlocked'); this.lastActivity = Date.now();
    this.idleTimer = setInterval(() => { if (Date.now() - this.lastActivity > IDLE_MS) { this.lock(); this.info.set('به‌دلیل بی‌فعالیتی قفل شد.'); } }, 10_000);
    this.reload();
  }

  reload(): void {
    if (!this.key) return;
    this.api.list().subscribe({
      next: async list => {
        const out: Row[] = [];
        let bad = 0;
        for (const e of list) {
          try { out.push({ id: e.id, modified: e.modified, data: { ...EMPTY, ...JSON.parse(await this.crypto.open(this.key!, e)) } }); }
          catch { bad++; }
        }
        this.rows.set(out);
        if (bad) this.error.set(`${bad} مورد رمزگشایی نشد.`);
      },
      error: e => this.error.set(e.message)
    });
  }

  activity(): void { this.lastActivity = Date.now(); }

  // ---- ویرایش
  newEntry(): void { this.draft.set({ ...EMPTY }); this.draftId.set(0); this.editing.set(true); this.selectedId.set(null); this.reveal.set(false); this.error.set(''); }
  edit(r: Row): void { this.draft.set({ ...r.data }); this.draftId.set(r.id); this.editing.set(true); this.reveal.set(false); this.error.set(''); }
  select(r: Row): void { this.selectedId.set(r.id); this.editing.set(false); this.reveal.set(false); }
  patch(field: keyof Secret, v: string): void { this.draft.update(d => ({ ...d, [field]: v })); }

  save(): void {
    if (!this.key) return;
    const d = this.draft();
    if (!d.title.trim()) return this.error.set('عنوان الزامی است.');
    this.busy.set(true);
    const data: Secret = { ...d, title: d.title.trim().slice(0, 200), url: d.url.trim().slice(0, 2000), category: d.category.trim().slice(0, 60), notes: d.notes.slice(0, 4000) };
    this.crypto.seal(this.key, JSON.stringify(data)).then(s => this.api.save({ id: this.draftId(), ...s }).subscribe({
      next: id => { this.busy.set(false); this.editing.set(false); this.selectedId.set(id); this.reload(); },
      error: e => this.fail(e.message)
    })).catch(e => this.fail(e.message));
  }

  remove(r: Row): void {
    if (!confirm(`«${r.data.title}» حذف شود؟`)) return;
    this.api.remove(r.id).subscribe({ next: () => { this.selectedId.set(null); this.reload(); }, error: e => this.error.set(e.message) });
  }

  genIntoDraft(): void {
    this.patch('password', this.crypto.generate(this.genLen(), { upper: this.genUpper(), lower: this.genLower(), digits: this.genDigits(), symbols: this.genSymbols() }));
    this.reveal.set(true);
  }

  // ---- کلیپ‌بورد با پاک‌سازی خودکار
  copy(text: string): void {
    this.activity();
    navigator.clipboard.writeText(text).then(() => {
      this.info.set('کپی شد؛ ۲۰ ثانیه دیگر از کلیپ‌بورد پاک می‌شود.');
      if (this.clipTimer) clearTimeout(this.clipTimer);
      this.clipTimer = setTimeout(() => navigator.clipboard.writeText('').catch(() => { }), CLIP_MS);
    }).catch(() => this.error.set('کپی ممکن نشد.'));
  }

  safeUrl(u: string): string | null {
    try { const x = new URL(u); return x.protocol === 'http:' || x.protocol === 'https:' ? x.href : null; } catch { return null; }
  }

  // ---- تغییر master password: همه‌ی ورودی‌ها با کلید جدید دوباره رمز می‌شوند
  rekey(): void {
    if (!this.key) return;
    if (this.newMaster().length < 10) return this.error.set('master password جدید حداقل ۱۰ کاراکتر باشد.');
    if (this.newMaster() !== this.newMaster2()) return this.error.set('تکرار master password جدید یکسان نیست.');
    if (!confirm('همهٔ ورودی‌ها با master password جدید رمز می‌شوند. ادامه می‌دهید؟')) return;
    this.busy.set(true);
    const salt = this.crypto.newSalt();
    const current = this.rows();
    (async () => {
      const k = await this.crypto.deriveKey(this.newMaster(), salt, VAULT_ITERATIONS);
      const v = await this.crypto.makeVerifier(k);
      const entries = [];
      for (const r of current) entries.push({ id: r.id, ...(await this.crypto.seal(k, JSON.stringify(r.data))) });
      this.api.rekey({ salt, iterations: VAULT_ITERATIONS, verifierCipher: v.cipher, verifierIv: v.iv, entries }).subscribe({
        next: () => {
          this.status = { salt, iterations: VAULT_ITERATIONS, verifierCipher: v.cipher, verifierIv: v.iv };
          this.key = k; this.newMaster.set(''); this.newMaster2.set(''); this.showChangeMaster.set(false);
          this.busy.set(false); this.info.set('master password تغییر کرد.'); this.reload();
        },
        error: e => this.fail(e.message)
      });
    })().catch(e => this.fail(e.message));
  }

  private clearMaster(): void { this.master.set(''); this.master2.set(''); }
  private fail(msg: string): void { this.busy.set(false); this.error.set(msg); }
}
