import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

export interface DataJobRun {
  id: string;
  sourceKey: string;
  marketKey: string;
  jobKey: string;
  trigger: string;
  status: string;
  timeZoneId?: string | null;
  startedAtUtc: string;
  finishedAtUtc?: string | null;
  processedCount: number;
  succeededCount: number;
  updatedCount: number;
  noDataCount: number;
  failedCount: number;
  errorIssueCount: number;
  remainingIssueCount: number;
  summary?: string | null;
}

export interface DataJobRunIssue {
  id: string;
  errorType: string;
  fingerprint: string;
  targetKeys: string[];
  message: string;
  occurrences: number;
  firstSeenAtUtc: string;
}

export interface DataJobRunDetail { run: DataJobRun; issues: DataJobRunIssue[]; }

@Injectable({ providedIn: 'root' })
export class DataMonitoringService {
  private readonly http = inject(HttpClient);

  private unwrap<T>(response: ApiResult<T>): T {
    if (!response.successed) throw new Error((response.errors ?? []).join(' ') || 'Request failed');
    return response.data;
  }

  recentRuns(): Observable<DataJobRun[]> {
    const params = new HttpParams().set('take', '250');
    return this.http.get<ApiResult<DataJobRun[]>>(apiUrl('DataMonitoring/Overview'), { params }).pipe(map(x => this.unwrap(x)));
  }

  runDetails(runId: string): Observable<DataJobRunDetail> {
    const params = new HttpParams().set('runId', runId);
    return this.http.get<ApiResult<DataJobRunDetail>>(apiUrl('DataMonitoring/RunDetails'), { params }).pipe(map(x => this.unwrap(x)));
  }

  deleteIssues(runId: string, issueIds: string[]): Observable<{ deletedCount: number }> {
    return this.http.post<ApiResult<{ deletedCount: number }>>(apiUrl('DataMonitoring/DeleteIssues'), { runId, issueIds }).pipe(map(x => this.unwrap(x)));
  }
}
