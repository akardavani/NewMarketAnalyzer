import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface Note { id: number; title: string; content: string; color?: string | null; lastModificationTime: string; }

@Injectable({ providedIn: 'root' })
export class NotesService {
  private readonly http = inject(HttpClient);

  private unwrap<T>(res: ApiResult<T>): T {
    if (!res.successed) throw new Error((res.errors ?? []).join(' ') || 'خطای نامشخص');
    return res.data;
  }

  getAll(): Observable<Note[]> {
    return this.http.get<ApiResult<Note[]>>(apiUrl('Notes/GetAll')).pipe(map(r => this.unwrap(r)));
  }
  create(title = '', content = '', color: string | null = null): Observable<number> {
    return this.http.post<ApiResult<number>>(apiUrl('Notes/Create'), { title, content, color }).pipe(map(r => this.unwrap(r)));
  }
  update(id: number, title: string, content: string, color?: string | null): Observable<number> {
    return this.http.post<ApiResult<number>>(apiUrl('Notes/Update'), { id, title, content, color }).pipe(map(r => this.unwrap(r)));
  }
  remove(id: number): Observable<number> {
    return this.http.post<ApiResult<number>>(apiUrl('Notes/Remove'), { id }).pipe(map(r => this.unwrap(r)));
  }
}
