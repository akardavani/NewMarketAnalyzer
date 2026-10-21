import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';
import { Sealed } from './vault-crypto.service';

export interface VaultStatus { initialized: boolean; salt?: string | null; iterations: number; verifierCipher?: string | null; verifierIv?: string | null; }
export interface EncryptedEntry extends Sealed { id: number; modified: string; }

@Injectable({ providedIn: 'root' })
export class PasswordsService {
  private readonly http = inject(HttpClient);

  private unwrap<T>(res: ApiResult<T>): T {
    if (!res.successed) throw new Error((res.errors ?? []).join(' ') || 'خطای نامشخص');
    return res.data;
  }
  private get<T>(path: string): Observable<T> {
    return this.http.get<ApiResult<T>>(apiUrl('Passwords/' + path)).pipe(map(r => this.unwrap(r)));
  }
  private post<T = number>(path: string, body: unknown): Observable<T> {
    return this.http.post<ApiResult<T>>(apiUrl('Passwords/' + path), body).pipe(map(r => this.unwrap(r)));
  }

  status(): Observable<VaultStatus> { return this.get<VaultStatus>('Status'); }
  list(): Observable<EncryptedEntry[]> { return this.get<EncryptedEntry[]>('GetAll'); }
  init(body: { salt: string; iterations: number; verifierCipher: string; verifierIv: string }): Observable<number> { return this.post('Init', body); }
  save(body: { id: number } & Sealed): Observable<number> { return this.post('Save', body); }
  remove(id: number): Observable<number> { return this.post('Remove', { id }); }
  rekey(body: { salt: string; iterations: number; verifierCipher: string; verifierIv: string; entries: ({ id: number } & Sealed)[] }): Observable<number> {
    return this.post('Rekey', body);
  }
}
