import { Injectable } from '@angular/core';

/**
 * رمزنگاری کاسه‌ی رمز داخل مرورگر با Web Crypto.
 * PBKDF2-SHA256 -> کلید AES-256-GCM (غیرقابل export). master password هرگز ذخیره یا ارسال نمی‌شود.
 */
export const VAULT_ITERATIONS = 600_000;
const VERIFIER_PLAIN = 'market-analyzer-vault-v1';

export interface Sealed { cipher: string; iv: string; }

export function toB64(bytes: ArrayBuffer | Uint8Array): string {
  const u = bytes instanceof Uint8Array ? bytes : new Uint8Array(bytes);
  let s = ''; for (let i = 0; i < u.length; i++) s += String.fromCharCode(u[i]);
  return btoa(s);
}
export function fromB64(b64: string): Uint8Array {
  const s = atob(b64); const u = new Uint8Array(s.length);
  for (let i = 0; i < s.length; i++) u[i] = s.charCodeAt(i);
  return u;
}

@Injectable({ providedIn: 'root' })
export class VaultCryptoService {
  private readonly enc = new TextEncoder();
  private readonly dec = new TextDecoder();

  /** کلید از master password و salt می‌سازد */
  async deriveKey(masterPassword: string, saltB64: string, iterations: number): Promise<CryptoKey> {
    const base = await crypto.subtle.importKey('raw', this.enc.encode(masterPassword), 'PBKDF2', false, ['deriveKey']);
    return crypto.subtle.deriveKey(
      { name: 'PBKDF2', salt: fromB64(saltB64) as BufferSource, iterations, hash: 'SHA-256' },
      base, { name: 'AES-GCM', length: 256 }, false, ['encrypt', 'decrypt']);
  }

  newSalt(): string { return toB64(crypto.getRandomValues(new Uint8Array(16))); }

  async seal(key: CryptoKey, plain: string): Promise<Sealed> {
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const buf = await crypto.subtle.encrypt({ name: 'AES-GCM', iv: iv as BufferSource }, key, this.enc.encode(plain));
    return { cipher: toB64(buf), iv: toB64(iv) };
  }

  async open(key: CryptoKey, s: Sealed): Promise<string> {
    const buf = await crypto.subtle.decrypt({ name: 'AES-GCM', iv: fromB64(s.iv) as BufferSource }, key, fromB64(s.cipher) as BufferSource);
    return this.dec.decode(buf);
  }

  /** تولید verifier برای بررسی درستی master password بدون ذخیره‌ی آن */
  async makeVerifier(key: CryptoKey): Promise<Sealed> { return this.seal(key, VERIFIER_PLAIN); }

  async checkVerifier(key: CryptoKey, v: Sealed): Promise<boolean> {
    try { return (await this.open(key, v)) === VERIFIER_PLAIN; } catch { return false; }
  }

  /** تولیدکننده‌ی رمز تصادفی با crypto.getRandomValues */
  generate(length: number, opts: { upper: boolean; lower: boolean; digits: boolean; symbols: boolean }): string {
    let pool = '';
    if (opts.lower) pool += 'abcdefghijkmnopqrstuvwxyz';
    if (opts.upper) pool += 'ABCDEFGHJKLMNPQRSTUVWXYZ';
    if (opts.digits) pool += '23456789';
    if (opts.symbols) pool += '!@#$%^&*()-_=+[]{};:,.?';
    if (!pool) pool = 'abcdefghijkmnopqrstuvwxyz23456789';
    const out: string[] = [];
    const rnd = new Uint32Array(length);
    crypto.getRandomValues(rnd);
    for (let i = 0; i < length; i++) out.push(pool[rnd[i] % pool.length]);
    return out.join('');
  }

  /** امتیاز ساده‌ی قدرت رمز: 0 (خیلی ضعیف) تا 4 (قوی) */
  strength(pw: string): number {
    if (!pw) return 0;
    let s = 0;
    if (pw.length >= 8) s++;
    if (pw.length >= 12) s++;
    if (/[a-z]/.test(pw) && /[A-Z]/.test(pw)) s++;
    if (/\d/.test(pw) && /[^A-Za-z0-9]/.test(pw)) s++;
    if (/(.)\1\1/.test(pw) || /^(password|123456|qwerty)/i.test(pw)) s = Math.min(s, 1);
    return Math.min(4, s);
  }
}
