import { Injectable, inject } from '@angular/core';
import { ReportFormat } from '../typings';
import { buildUrl, saveFile } from '../utils';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ReportService {

  private readonly httpClient: HttpClient;

  constructor() {
    this.httpClient = inject(HttpClient);
  }

  exportExcel(reportPath: string, filterData: any): Observable<any> {
    return this.export(ReportFormat.Excel, reportPath, filterData);
  }

  exportPdf(reportPath: string, filterData: any): Observable<any> {
    return this.export(ReportFormat.Pdf, reportPath, filterData);
  }

  export(format: ReportFormat, reportPath: string, filterData: any): Observable<void> {
    const url = buildUrl(reportPath, Object.assign({ reportFormat: format }, filterData));
    return this.httpClient
      .get<any>(url, { observe: 'response', responseType: 'arraybuffer' as any })
      .pipe(
        tap(saveFile),
        map(() => void (0))
      );
  }

}
