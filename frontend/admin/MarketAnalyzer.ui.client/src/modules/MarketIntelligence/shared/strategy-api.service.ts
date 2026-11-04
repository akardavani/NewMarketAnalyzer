import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface SymbolItem { symbolKey: string; symbol: string; name: string; market: string; }
export interface RunRatioCandlesRequest { symbols: string[]; startDate: string; endDate: string; }
export interface RunIranQueueRequest { inputFilePath: string; fromRatio: number; toRatio: number; return1Month: number; }
export interface RunBottomReversalRequest { inputFilePath: string; startDate: string; endDate: string; }
export interface RunMultiIndicatorDivergenceRequest { inputFilePath: string; }
export interface RunYtmFutureRequest { symbol: string; calculates: string; startDate: string; endDate: string; }
export interface RunIranEquityMainRequest { inputFilePath: string; }

/** Typed calls for each strategy; the generic method remains for compatibility. */
@Injectable({ providedIn: 'root' })
export class StrategyApiService {
  private readonly http = inject(HttpClient);

  private unwrap<T>(response: ApiResult<T>): T {
    if (!response.successed) throw new Error((response.errors ?? []).join(' ') || 'خطای نامشخص');
    return response.data;
  }

  /** Returns the strategy payload. */
  run(strategy: string, parameters: Record<string, unknown>): Observable<any> {
    return this.http.post<ApiResult<any>>(apiUrl('Strategy/Run'), { strategy, parameters }).pipe(
      map(response => {
        const result = this.unwrap(response);
        return result && typeof result === 'object' && 'data' in result ? result.data : result;
      })
    );
  }

  runRatioCandles(request: RunRatioCandlesRequest): Observable<any> {
    return this.http.post<ApiResult<any>>(apiUrl('Strategy/RunRatioCandles'), request).pipe(map(response => this.unwrap(response)));
  }

  runIranQueue(request: RunIranQueueRequest): Observable<any> {
    return this.http.post<ApiResult<any>>(apiUrl('Strategy/RunIranQueue'), request).pipe(map(response => this.unwrap(response)));
  }

  runBottomReversal(request: RunBottomReversalRequest): Observable<any> {
    return this.http.post<ApiResult<any>>(apiUrl('Strategy/RunBottomReversal'), request).pipe(map(response => this.unwrap(response)));
  }

  runMultiIndicatorDivergence(request: RunMultiIndicatorDivergenceRequest): Observable<any> {
    return this.http.post<ApiResult<any>>(apiUrl('Strategy/RunMultiIndicatorDivergence'), request).pipe(map(response => this.unwrap(response)));
  }

  runYtmFuture(request: RunYtmFutureRequest): Observable<any> {
    return this.http.post<ApiResult<any>>(apiUrl('Strategy/RunYtmFuture'), request).pipe(map(response => this.unwrap(response)));
  }

  runIranEquityMain(request: RunIranEquityMainRequest): Observable<any> {
    return this.http.post<ApiResult<any>>(apiUrl('Strategy/RunIranEquityMain'), request).pipe(map(response => this.unwrap(response)));
  }

  searchSymbols(text: string): Observable<SymbolItem[]> {
    return this.http.get<ApiResult<SymbolItem[]>>(apiUrl('Strategy/SearchSymbols'), { params: { text } })
      .pipe(map(response => response.successed ? response.data : []));
  }
}
