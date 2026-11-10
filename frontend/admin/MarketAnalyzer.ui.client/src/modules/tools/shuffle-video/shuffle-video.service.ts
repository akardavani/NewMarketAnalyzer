import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface ShuffleRequest { sourcePath: string; outputPath: string; prefix: string; folderCount: number; targetMinutes: number; }
export interface ShuffleFile { path: string; category: string; bytes: number; seconds: number; }
export interface ShuffleFolder { name: string; seconds: number; complete: boolean; files: ShuffleFile[]; }
export interface ShufflePlan {
  id: string; expiresUtc: string; sourcePath: string; outputPath: string; targetMinutes: number;
  availableFiles: number; previouslyUsed: number; totalBytes: number; folders: ShuffleFolder[]; warnings: string[];
}
export interface ShuffleResult { path: string; copiedFiles: number; copiedBytes: number; }
@Injectable({ providedIn: 'root' })
export class ShuffleVideoService {
  private readonly http = inject(HttpClient);
  private unwrap<T>(res: ApiResult<T>): T {
    if (!res.successed) throw new Error((res.errors ?? []).join(' ') || 'خطای نامشخص');
    return res.data;
  }
  preview(request: ShuffleRequest) {
    return this.http.post<ApiResult<ShufflePlan>>(apiUrl('ShuffleVideo/Preview'), request).pipe(map(r => this.unwrap(r)));
  }
  run(planId: string, allowIncomplete: boolean) {
    return this.http.post<ApiResult<ShuffleResult>>(apiUrl('ShuffleVideo/Run'), { planId, allowIncomplete }).pipe(map(r => this.unwrap(r)));
  }
}
