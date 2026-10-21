import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export type BookmarkKind = 0 | 1 | 2 | 3 | 4;
export const KIND_LABELS: Record<number, string> = { 0: 'لینک', 1: 'PDF', 2: 'تصویر', 3: 'ویدیو', 4: 'سند' };

export interface BookmarkItem {
  id: number; collectionId: number | null; url: string; title: string; note: string; imageUrl?: string | null;
  kind: BookmarkKind; isFavorite: boolean; tags: string[]; lastModificationTime: string;
}
export interface CollectionItem { id: number; parentId: number | null; name: string; count: number; }
export interface TagItem { name: string; count: number; }
export interface UrlMetadata { url: string; title: string; imageUrl?: string | null; kind: BookmarkKind; }
export interface BookmarkFilter {
  search?: string; tag?: string; collectionId?: number | null; favoritesOnly?: boolean;
  kind?: number | null; from?: string; to?: string; skip: number; take: number;
}
export interface SaveBookmark {
  id: number; url: string; title: string; note: string; imageUrl?: string | null;
  kind: BookmarkKind; collectionId: number | null; tags: string[];
}

@Injectable({ providedIn: 'root' })
export class BookmarksService {
  private readonly http = inject(HttpClient);

  private unwrap<T>(res: ApiResult<T>): T {
    if (!res.successed) throw new Error((res.errors ?? []).join(' ') || 'خطای نامشخص');
    return res.data;
  }
  private get<T>(path: string, params: HttpParams = new HttpParams()): Observable<T> {
    return this.http.get<ApiResult<T>>(apiUrl('Bookmarks/' + path), { params }).pipe(map(r => this.unwrap(r)));
  }
  private post<T = number>(path: string, body: unknown): Observable<T> {
    return this.http.post<ApiResult<T>>(apiUrl('Bookmarks/' + path), body).pipe(map(r => this.unwrap(r)));
  }

  getAll(f: BookmarkFilter): Observable<BookmarkItem[]> {
    let p = new HttpParams().set('skip', f.skip).set('take', f.take).set('favoritesOnly', !!f.favoritesOnly);
    if (f.search) p = p.set('search', f.search);
    if (f.tag) p = p.set('tag', f.tag);
    if (f.collectionId) p = p.set('collectionId', f.collectionId);
    if (f.kind !== null && f.kind !== undefined) p = p.set('kind', f.kind);
    if (f.from) p = p.set('from', f.from);
    if (f.to) p = p.set('to', f.to);
    return this.get<BookmarkItem[]>('GetAll', p);
  }
  collections(): Observable<CollectionItem[]> { return this.get('Collections'); }
  tags(term = ''): Observable<TagItem[]> { return this.get('Tags', new HttpParams().set('term', term)); }
  preview(url: string): Observable<UrlMetadata> { return this.get('Preview', new HttpParams().set('url', url)); }
  save(b: SaveBookmark): Observable<number> { return this.post('Save', b); }
  setFavorite(id: number, value: boolean): Observable<number> { return this.post('SetFavorite', { id, value }); }
  remove(id: number): Observable<number> { return this.post('Remove', { id }); }
  saveCollection(id: number, name: string, parentId: number | null): Observable<number> { return this.post('SaveCollection', { id, name, parentId }); }
  removeCollection(id: number): Observable<number> { return this.post('RemoveCollection', { id }); }
  renameTag(oldName: string, newName: string): Observable<number> { return this.post('RenameTag', { old: oldName, new: newName }); }
  removeTag(name: string): Observable<number> { return this.post('RemoveTag', { name }); }
}
