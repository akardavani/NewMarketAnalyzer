import { IServerSideDatasource, IServerSideGetRowsParams, IServerSideGetRowsRequest } from 'ag-grid-community';
import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable, Injector, inject } from '@angular/core';
import { BehaviorSubject, catchError, Observable, Subject, tap, throwError } from 'rxjs';
import { ApiResult, PlainObject, QueryResult, SortType } from '../typings';
import { buildUrl, providedInRoot } from '../utils';

export interface IDataSourceService<TData = void, TFilter = void, TResponse = TData> {
  data: Observable<TData[]>;
  filterData: TFilter | undefined;
  requestStarted: EventEmitter<void>;
  requestCompleted: EventEmitter<void>;
  serverResponded: EventEmitter<TResponse>;
}

export abstract class DataSourceServiceBase<TData = void, TFilter = void, TResponse = TData> implements IDataSourceService<TData, TFilter, TResponse> {

  filterData: TFilter | undefined;
  requestStarted = new EventEmitter<void>();
  requestCompleted = new EventEmitter<void>();
  serverResponded = new EventEmitter<TResponse>();
  data: Observable<TData[]> = new BehaviorSubject<TData[]>([]);

  protected abstract url: string;
  protected readonly httpClient: HttpClient;

  constructor() {
    this.httpClient = inject(HttpClient);
    if (providedInRoot(this)) {
      throw new Error(`${this.constructor.name} should not be provided in root!`);
    }
  }

}

@Injectable()
export abstract class ServerSideDataSourceService<TData = void, TFilter = void, TResponse extends QueryResult<TData> = QueryResult<TData>>
  extends DataSourceServiceBase<TData, TFilter, TResponse> implements IServerSideDatasource {

  getRows(rowParams: IServerSideGetRowsParams<TData, any>): void {
    this.requestStarted.emit();

    const queryParams = Object.assign({
      page: rowParams.api.paginationGetCurrentPage() + 1,
      orderBy: this.mapOrderBy(rowParams.request),
      rows: rowParams.api.paginationGetPageSize()
    }, this.filterData);

    const url = buildUrl(this.url, queryParams);

    this.httpClient
      .get<ApiResult<TResponse>>(url)
      .pipe(
        tap(() => this.requestCompleted.emit()),
        catchError((error) => {
          this.requestCompleted.emit();
          return throwError(() => error);
        })
      )
      .subscribe(response => {
        if (response.successed) {
          this.serverResponded.emit(response.data);
          rowParams.success({
            rowData: response.data.rows ?? [],
            rowCount: response.data.itemsCount ?? 0
          });

          (this.data as Subject<TData[]>).next(response.data.rows);
        }
        else {
          rowParams.fail();
        }
      });
  }

  private mapOrderBy(request: IServerSideGetRowsRequest): PlainObject<SortType> | undefined {
    if (!request.sortModel.length) {
      return undefined;
    }

    const rx = /(\w+)_\d+/;
    const result: PlainObject<SortType> = {};

    request.sortModel.forEach(item => {
      const sortType = item.sort == 'asc' ?
        SortType.ASC :
        SortType.DESC;

      result[item.colId.replace(rx, '$1')] = sortType;
    });

    return result;
  }
}

export abstract class ClientSideDataSourceService<TData = void, TFilter = void, TResponse = void> extends DataSourceServiceBase<TData, TFilter, TResponse> {

  orderBy?: PlainObject<SortType>;

  refresh(): void {
    this.requestStarted.emit();

    const data = Object.assign({}, this.filterData, this.orderBy);
    this.httpClient
      .get<ApiResult<TResponse>>(buildUrl(this.url, data))
      .pipe(
        tap(() => this.requestCompleted.emit()),
        catchError((error) => {
          this.requestCompleted.emit();
          return throwError(() => error);
        })
      )
      .subscribe((response: ApiResult<TResponse>) => {
        if (response.successed) {
          this.serverResponded.emit(response.data);
          (this.data as Subject<TData[]>).next(this.extractRowsData(response.data));
        }
      });
  }

  extractRowsData(response: TResponse): TData[] {
    if (Array.isArray(response)) {
      return response as any;
    }
    else {
      throw new Error('Not supported Response type.');
    }
  }

}
