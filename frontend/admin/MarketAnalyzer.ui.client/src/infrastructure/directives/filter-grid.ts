import { Directive, inject } from "@angular/core";
import { GridBase } from "./grid-base";
import { FormBuilder, FormGroup } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import {
  IDataSourceService,
  ServerSideDataSourceService,
  ClientSideDataSourceService
} from "@services/data-source.service";
import { GridReadyEvent, PaginationChangedEvent, RowModelType, SortChangedEvent } from "ag-grid-enterprise";
import { PlainObject, QueryResult, SortType } from "@typings";
import { Observable, of } from "rxjs";
import { bind, getValue, setValue } from "@utils";


@Directive()
export abstract class FilterGridBase<TData = void, TFilter = void, TResponse = void> extends GridBase<TData> {

  protected _initFilter: any;
  protected readonly filter: FormGroup;
  protected readonly httpClient: HttpClient;

  constructor(dataSource?: IDataSourceService<TData, TFilter, TResponse>) {
    super();

    this.httpClient = inject(HttpClient);
    this.filter = this.buildFilter(inject(FormBuilder));
    this._initFilter = Object.assign({}, this.filter.getRawValue());
    this.dataSource = dataSource;

  }

  get dataSource(): IDataSourceService<TData, TFilter, TResponse> | undefined {
    return getValue(this, 'dataSource');
  }
  set dataSource(value: IDataSourceService<TData, TFilter, TResponse> | undefined) {
    if (this.dataSource != value) {
      setValue(this, 'dataSource', value);
      if (value) {
        value.filterData = this.filter.getRawValue();
      }

      this.onDataSourceChanged();
    }
  }

  resetFilter(): void {
    this.filter.reset(this._initFilter);
    this.applyFilter();
  }

  protected onDataSourceChanged(): void {

  }

  abstract applyFilter(): void;

  protected abstract buildFilter(builder: FormBuilder): FormGroup;

}

@Directive()
export abstract class ServerSideFilterGrid<TData = void, TFilter = void, TResponse extends QueryResult<TData> = QueryResult<TData>>
  extends FilterGridBase<TData, TFilter, TResponse> {

  _dataSourceAttached = false;
  override rowModelType: RowModelType = 'serverSide';

  constructor(dataSource: ServerSideDataSourceService<TData, TFilter, TResponse>) {
    super(dataSource);
    this.dataSource = dataSource;
  }

  override onGridReady(event: GridReadyEvent<TData>): void {
    super.onGridReady(event);
    event.api.setGridOption('onPaginationChanged', this.onPaginationChanged);

    if (!this.deferLoad && this.dataSource) {
      this.attachDataSource();
      this.applyFilter();
    }
  }

  override applyFilter(): void {
    if (this.filter.valid && this.dataSource) {
      if (!this._dataSourceAttached) {
        this.attachDataSource();
      }

      this.dataSource.filterData = this.filter.getRawValue();
      this.gridApi?.refreshServerSide({ purge: true });
    }
  }

  override refresh(): void {
    this.gridApi!.refreshServerSide({ purge: true });
  }

  @bind()
  onPaginationChanged(event: PaginationChangedEvent<TData>): void {
    if (event.newPageSize) {
      event.api.paginationGoToFirstPage();
      this.refresh();
    }
  }

  protected attachDataSource(): void {
    this._dataSourceAttached = true;
    this.gridApi!.setGridOption('serverSideDatasource', as<ServerSideDataSourceService>(this.dataSource));
    this.dataSource!.serverResponded.subscribe(this.autoSizeColumns);
  }

}

@Directive()
export abstract class ClientSideFilterGrid<TData = void, TFilter = void, TResponse = TData[]> extends FilterGridBase<TData, TFilter, TResponse> {

  data: Observable<TData[]>;

  override pagination = false;
  override rowModelType: RowModelType = 'clientSide';

  constructor(dataSource: ClientSideDataSourceService<TData, TFilter, TResponse>) {
    super(dataSource);
    if (dataSource) {
      this.data = dataSource.data;
      dataSource.filterData = this.filter.getRawValue();
    }
    else {
      this.data = of<TData[]>([]);
    }
  }

  override onDataSourceChanged(): void {
    if (this.dataSource) {
      this.dataSource.requestStarted.subscribe(() => this.setLoader(true));
      this.dataSource.requestCompleted.subscribe(() => this.setLoader(false));
    }
  }

  override onGridReady(event: GridReadyEvent<TData>): void {
    super.onGridReady(event);
    event.api.setGridOption('onSortChanged', this.onSortChanged);
    if (!this.deferLoad && this.dataSource) {
      this.applyFilter();
    }
  }

  override applyFilter(): void {
    if (this.filter.valid && this.dataSource) {
      this.dataSource.filterData = this.filter.getRawValue();
      as<ClientSideDataSourceService>(this.dataSource).refresh();
    }
  }

   override refresh(): void {
    as<ClientSideDataSourceService>(this.dataSource).refresh();
  }

  protected setLoader(value: boolean): void {
    this.gridApi?.setGridOption('loading', value);
  }

  @bind()
  private onSortChanged(event: SortChangedEvent<TData>): void {
    const orderBy = event.api
      .getColumnState()
      .filter(e => !!e.sort)
      .sort((a, b) => (a.sortIndex || 0) - (b.sortIndex || 0))
      .reduce((model, col) => {
        model[col.colId] = col.sort == 'asc' ? SortType.ASC : SortType.DESC;
        return model;
      }, <PlainObject<SortType>>{});

    as<ClientSideDataSourceService>(this.dataSource).orderBy = orderBy;
  }

}

function as<T>(source: any): T {
  return source as T;
}
