import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { catchError, from, map, mergeMap, Observable, throwError } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface MarketCapability {
  capability: number;
  activeProviderKey: string | null;
  availableProviderKeys: string[];
}

export interface MarketOverview {
  key: string;
  name: string;
  persianName: string;
  currencyCode: string;
  isActive: boolean;
  capabilities: MarketCapability[];
  settings: Record<string, string>;
}

export interface SettingItem { scope: string; key: string; value: string; }
export interface MarketJobResult {
  marketKey: string;
  job: number;
  status: number;
  processed: number;
  updated: number;
  noData: number;
  fullyRefetched: number;
  failed: number;
  message?: string | null;
  errorSamples: string[];
  duration: string;
}

@Injectable({ providedIn: 'root' })
export class MarketDataSettingsService {
  private readonly http = inject(HttpClient);

  private unwrap<T>(response: ApiResult<T>): T {
    if (!response.successed) throw new Error((response.errors ?? []).join(' ') || 'Request failed');
    return response.data;
  }

  overview(): Observable<MarketOverview[]> {
    return this.http.get<ApiResult<MarketOverview[]>>(apiUrl('MarketDataSettings/Overview')).pipe(map(r => this.unwrap(r)));
  }

  providerSettings(providerKey: string): Observable<SettingItem[]> {
    const params = new HttpParams().set('providerKey', providerKey);
    return this.http.get<ApiResult<SettingItem[]>>(apiUrl('MarketDataSettings/ProviderSettings'), { params }).pipe(map(r => this.unwrap(r)));
  }

  setSetting(scope: string, key: string, value: string): Observable<void> {
    return this.http.post<ApiResult<void>>(apiUrl('MarketDataSettings/SetSetting'), { scope, key, value }).pipe(map(r => this.unwrap(r)));
  }

  switchProvider(marketKey: string, capability: number, providerKey: string, confirmWipe: boolean): Observable<unknown> {
    return this.http.post<ApiResult<unknown>>(apiUrl('MarketDataSettings/SwitchProvider'), { marketKey, capability, providerKey, confirmWipe }).pipe(map(r => this.unwrap(r)));
  }

  runJob(marketKey: string, job: number): Observable<MarketJobResult> {
    return this.http.post<ApiResult<MarketJobResult>>(apiUrl('MarketDataSettings/RunJob'), { marketKey, job }).pipe(map(r => this.unwrap(r)));
  }

  refreshCatalogs(): Observable<Blob> {
    return this.http.post(apiUrl('MarketDataSettings/RefreshCatalogs'), null, { responseType: 'blob' }).pipe(
      catchError((error: HttpErrorResponse) => {
        if (!(error.error instanceof Blob)) return throwError(() => error);
        return from(error.error.text()).pipe(mergeMap(body => {
          let message = body || error.message;
          try {
            const payload = JSON.parse(body);
            message = payload.errors?.join(' ') || payload.message || message;
          } catch { /* Keep the plain response body for non-JSON errors. */ }
          return throwError(() => new Error(message));
        }));
      })
    );
  }
}
