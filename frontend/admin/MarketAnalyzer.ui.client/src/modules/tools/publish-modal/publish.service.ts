import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface PublishFolderItem { name: string; path: string; }

export interface PublishTargetItem {
  id: number; key: string; title: string;
  folders: PublishFolderItem[]; outputPath: string; displayOrder: number; isActive: boolean; exists?: boolean;
}
export interface PublishResult { path: string; size: number; }

@Injectable({ providedIn: 'root' })
export class PublishService {
  private readonly http = inject(HttpClient);

  private unwrap<T>(res: ApiResult<T>): T {
    if (!res.successed) throw new Error((res.errors ?? []).join(' ') || 'خطای نامشخص');
    return res.data;
  }
  getAll(): Observable<PublishTargetItem[]> {
    return this.http.get<ApiResult<PublishTargetItem[]>>(apiUrl('Publish/GetAll')).pipe(map(r => this.unwrap(r)));
  }
  save(t: Omit<PublishTargetItem, 'exists'>): Observable<number> {
    return this.http.post<ApiResult<number>>(apiUrl('Publish/Save'), t).pipe(map(r => this.unwrap(r)));
  }
  remove(id: number): Observable<number> {
    return this.http.post<ApiResult<number>>(apiUrl('Publish/Remove'), { id }).pipe(map(r => this.unwrap(r)));
  }
  run(keys: string[]): Observable<PublishResult> {
    return this.http.post<ApiResult<PublishResult>>(apiUrl('Publish/Run'), { keys }).pipe(map(r => this.unwrap(r)));
  }
}
