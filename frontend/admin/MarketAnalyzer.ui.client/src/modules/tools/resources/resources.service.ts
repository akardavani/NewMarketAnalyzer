import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface ResourceFile { id: number; fileName: string; contentType: string; size: number; }
export interface ResourceItem { id: number; title: string; description: string; link?: string | null; tags: string[]; files: ResourceFile[]; lastModificationTime: string; }

@Injectable({ providedIn: 'root' })
export class ResourcesService {
  private readonly http = inject(HttpClient);

  private unwrap<T>(res: ApiResult<T>): T {
    if (!res.successed) throw new Error((res.errors ?? []).join(' ') || 'خطای نامشخص');
    return res.data;
  }

  getAll(search: string, skip: number, take = 20): Observable<ResourceItem[]> {
    const params = new HttpParams().set('search', search).set('skip', skip).set('take', take);
    return this.http.get<ApiResult<ResourceItem[]>>(apiUrl('Resources/GetAll'), { params }).pipe(map(r => this.unwrap(r)));
  }
  tags(term: string): Observable<string[]> {
    return this.http.get<ApiResult<string[]>>(apiUrl('Resources/Tags'), { params: new HttpParams().set('term', term) }).pipe(map(r => this.unwrap(r)));
  }
  save(r: { id: number; title: string; description: string; link: string; tags: string[] }): Observable<number> {
    return this.http.post<ApiResult<number>>(apiUrl('Resources/Save'), r).pipe(map(x => this.unwrap(x)));
  }
  remove(id: number): Observable<number> {
    return this.http.post<ApiResult<number>>(apiUrl('Resources/Remove'), { id }).pipe(map(x => this.unwrap(x)));
  }
  uploadFile(resourceId: number, fileId: number, file: File): Observable<ResourceFile> {
    const fd = new FormData();
    fd.append('resourceId', String(resourceId));
    fd.append('fileId', String(fileId));
    fd.append('file', file, file.name);
    return this.http.post<ApiResult<ResourceFile>>(apiUrl('Resources/UploadFile'), fd).pipe(map(x => this.unwrap(x)));
  }
  removeFile(id: number): Observable<number> {
    return this.http.post<ApiResult<number>>(apiUrl('Resources/RemoveFile'), { id }).pipe(map(x => this.unwrap(x)));
  }
  download(f: ResourceFile): Observable<Blob> {
    return this.http.get(apiUrl('Resources/DownloadFile'), { params: new HttpParams().set('id', f.id), responseType: 'blob' });
  }
}
