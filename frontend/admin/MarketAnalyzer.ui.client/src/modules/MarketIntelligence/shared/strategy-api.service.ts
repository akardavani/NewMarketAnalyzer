import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface SymbolItem { symbolKey: string; symbol: string; name: string; market: string; }

/** All strategies go through StrategyController: POST Strategy/Run. */
@Injectable({ providedIn: 'root' })
export class StrategyApiService {
  private readonly http = inject(HttpClient);

  /** Returns AnalysisResult.Data (the strategy specific payload). Throws Error(message) on failure. */
  run(strategy: string, parameters: Record<string, unknown>): Observable<any> {
    return this.http.post<ApiResult<any>>(apiUrl('Strategy/Run'), { strategy, parameters }).pipe(
      map(res => {
        if (!res.successed) throw new Error((res.errors ?? []).join(' ') || 'خطای نامشخص');
        const r = res.data;
        return r && typeof r === 'object' && 'data' in r ? r.data : r;
      })
    );
  }

  searchSymbols(text: string): Observable<SymbolItem[]> {
    return this.http.get<ApiResult<SymbolItem[]>>(apiUrl('Strategy/SearchSymbols'), { params: { text } })
      .pipe(map(res => (res.successed ? res.data : [])));
  }
}
