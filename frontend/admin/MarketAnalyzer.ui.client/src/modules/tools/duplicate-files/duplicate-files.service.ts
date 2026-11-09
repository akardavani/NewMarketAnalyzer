import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface DuplicateGroup { hash: string; size: number; paths: string[]; }
export interface DuplicateScanResult {
  scannedFiles: number; hashedFiles: number; skippedEntries: number;
  duplicateBytes: number; groups: DuplicateGroup[]; warnings: string[];
}
@Injectable({ providedIn: 'root' })
export class DuplicateFilesService {
  private readonly http = inject(HttpClient);
  scan(paths: string[], recursive: boolean) {
    return this.http.post<ApiResult<DuplicateScanResult>>(apiUrl('DuplicateFiles/Scan'), { paths, recursive }).pipe(
      map(res => {
        if (!res.successed) throw new Error((res.errors ?? []).join(' ') || 'خطای جست‌وجو');
        return res.data;
      })
    );
  }
}
