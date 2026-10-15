import { ChangeDetectorRef, Directive, Injector, inject } from '@angular/core';
import {
  ClientSideRowModelModule,
  ColDef,
  GridApi,
  GridReadyEvent,
  ModuleRegistry,
  RowModelType,
  Theme,
  ThemeDefaultParams,
  ValidationModule,
  LocaleModule,
  PaginationModule,
  RowSelectionOptions,
  StatusPanelDef,
  PinnedRowModule,
  GetRowIdParams,
  TooltipModule,
  SizeColumnsToFitGridStrategy,
  SizeColumnsToFitProvidedWidthStrategy,
  SizeColumnsToContentStrategy,
  ColumnAutoSizeModule,
  ColumnApiModule,
  CellStyleModule,
  RowSelectionModule,
  RowStyleModule,
  RenderApiModule,
  RowAutoHeightModule,
} from 'ag-grid-community';
import {
  LicenseManager,
  ServerSideRowModelModule,
  ServerSideRowModelApiModule,
  StatusBarModule,
} from 'ag-grid-enterprise';
import { AG_GRID_LOCALE_IR } from '@ag-grid-community/locale';
import { GrowlService } from '@services/growl.service';
import { defaultGridTheme } from './grid-theme';
import { boldPinnedCss, isIdOwner } from '@grid-utils';
import { bind, buildUrl } from '@utils';
import { LoaderComponent } from '@components/loader/loader.component';
import { DataNormalizer, FormGroupOf, IRefreshableHost } from '@form-utils';
import {
  catchError,
  finalize,
  Observable,
  Subject,
  takeUntil,
  throwError,
} from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { FormArray, FormBuilder } from '@angular/forms';
import { ApiResult, Nullable } from '@typings';
import { MatDialogRef } from '@angular/material/dialog';

LicenseManager.setLicenseKey(
  'IRDEVELOPERS_COM_NDEwMjM0NTgwMDAwMA==f08aae16269f416fe8d65bbf9396be5f',
);
ModuleRegistry.registerModules([
  LocaleModule,
  TooltipModule,
  RowStyleModule,
  CellStyleModule,
  PinnedRowModule,
  StatusBarModule,
  ColumnApiModule,
  RenderApiModule,
  ValidationModule,
  PaginationModule,
  RowSelectionModule,
  RowAutoHeightModule,
  ColumnAutoSizeModule,
  ClientSideRowModelModule,
  ServerSideRowModelModule,
  ServerSideRowModelApiModule,
]);

export const agGridLocalizedText = {
  ...AG_GRID_LOCALE_IR,

  pageSizeSelectorLabel: 'تعداد سطر',
};

@Directive()
export abstract class GridBase<TData = void> implements IRefreshableHost {
  deferLoad = false;
  showSidebar = false;

  width = '100%';
  height = '100%';
  localeText = agGridLocalizedText;
  theme: Theme<ThemeDefaultParams> = defaultGridTheme;
  defaultColDef: ColDef<TData> = {
    width: 100,
    cellDataType: false,
    enableValue: false,
    enableRowGroup: false,
    enablePivot: false,
    editable: false,
    sortable: true,
    resizable: true,
    filter: false,
    suppressMovable: true,
    cellClass: boldPinnedCss,
  };
  autoSizeStrategy: AutoSizeStrategy = { type: 'fitCellContents' };
  rowSelection: RowSelectionOptions<TData> = {
    mode: 'singleRow',
    checkboxes: false,
    enableClickSelection: true,
    enableSelectionWithoutKeys: true,
  };
  statusBar: { statusPanels: StatusPanelDef[] } = {
    statusPanels: [],
  };
  gridApi?: GridApi<TData>;
  loadingOverlayComponent = LoaderComponent;
  pagination: boolean = true;
  paginationPageSize: number = 100;
  paginationPageSizeSelector = [50, 100, 200, 500, 1000, 10000, 100000];
  //paginationNumberFormatter

  abstract readonly rowModelType: RowModelType;
  abstract readonly columnDefs: ColDef<TData>[];

  protected readonly injector: Injector;
  protected readonly growl: GrowlService;
  protected readonly cdr: ChangeDetectorRef;

  constructor() {
    this.injector = inject(Injector);
    this.growl = inject(GrowlService);
    this.cdr = inject(ChangeDetectorRef);
  }

  get selectedRows(): TData[] {
    return this.gridApi?.getSelectedRows() || [];
  }

  onGridReady(event: GridReadyEvent<TData>): void {
    this.gridApi = event.api;

    event.api.setGridOption('theme', this.theme);
    event.api.setGridOption('statusBar', this.statusBar);
    event.api.setGridOption('columnDefs', this.columnDefs);
    event.api.setGridOption('pagination', this.pagination);
    event.api.setGridOption('rowSelection', this.rowSelection);
    event.api.setGridOption('defaultColDef', this.defaultColDef);
    event.api.setGridOption('paginationPageSize', this.paginationPageSize);

    event.api.setGridOption('animateRows', false);
    event.api.setGridOption('enableCellTextSelection', true);
    event.api.setGridOption('onRowSelected', () => this.cdr.markForCheck());
  }

  toggleSidebar() {
    this.showSidebar = !this.showSidebar;
    this.cdr.markForCheck();
  }

  getRowId(params: GetRowIdParams<TData>): string {
    if (params.rowPinned == 'bottom') {
      return 'PinnedRowBottom_0';
    } else if (params.rowPinned == 'top') {
      return 'PinnedRowTop_0';
    } else if (!isIdOwner(params.data)) {
      throw new Error(
        'Not supported `getRowId` row data. Override `getRowId` method in drived class.',
      );
    }

    return typeof params.data.id === 'string'
      ? params.data.id
      : String(params.data.id);
  }

  @bind()
  autoSizeColumns(): void {
    const columns = this.gridApi!.getAllGridColumns()!.map((col) =>
      col.getId(),
    );
    requestAnimationFrame(() => this.gridApi!.autoSizeColumns(columns));
  }

  abstract refresh(): void;
}

export abstract class PlainGrid<TData = void> extends GridBase<TData> {
  override pagination: boolean = false;
  override rowModelType: RowModelType = 'clientSide';

  constructor() {
    super();
    this.defaultColDef.sortable = false;
  }

  override refresh(): void {
    throw new Error('Not supported.');
  }
}

@Directive()
export abstract class FormGridBase<
  TModel = void,
  TData = void,
> extends PlainGrid<TData> {
  loading = false;
  readonly formGroup: FormGroupOf<TModel>;

  protected cancellation?: Subject<void>;
  protected readonly _httpClient: HttpClient;
  protected readonly _dialog: MatDialogRef<FormGridBase<TModel>>;

  protected _initValue: TModel;

  constructor() {
    super();
    this._httpClient = inject(HttpClient);
    this.formGroup = this.buildForm(inject(FormBuilder));
    this._initValue = Object.assign({}, this.formValue);
    this._dialog = inject(MatDialogRef<FormGridBase<TModel>>);
  }

  protected get formValue(): TModel {
    return this.formGroup.getRawValue() as TModel;
  }

  submit(): void {
    if (this.formGroup.valid) {
      this.submitCore();
    }
  }

  ngOnDestroy(): void {
    if (this.cancellation) {
      this.cancellation.next();
      this.cancellation.complete();
      this.cancellation = undefined;
    }
  }

  close(): void {
    this._dialog.close();
  }

  protected getData<TResponse, TData = any>(
    url: string,
    data?: TData,
  ): Observable<ApiResult<TResponse>> {
    if (!this.cancellation) {
      this.cancellation = new Subject<void>();
    }

    return this._httpClient
      .get<ApiResult<TResponse>>(buildUrl(url, data))
      .pipe(takeUntil(this.cancellation));
  }

  protected postData<TResponse = void, TResult = TModel>(
    url: string,
    normalizer?: DataNormalizer<TModel, TResult>,
  ): Observable<ApiResult<TResponse>> {
    this.setLoader(true);
    if (!this.cancellation) {
      this.cancellation = new Subject<void>();
    }

    const requestData = normalizer
      ? normalizer(this.formValue)
      : this.formValue;

    return this._httpClient.post<ApiResult<TResponse>>(url, requestData).pipe(
      takeUntil(this.cancellation),
      //tap(() => this.setLoader(false)),
      catchError((error) => {
        this.setLoader(false);
        return throwError(() => error);
      }),
      finalize(() => this.setLoader(false)),
    );
  }

  protected reset(): void {
    this.formGroup.reset(this._initValue as any);
  }

  protected subscribeChanges<K extends keyof TModel>(
    property: K,
    callback: (value: TModel[K]) => void,
  ): void {
    this.formGroup.controls[property].valueChanges.subscribe(callback);
  }

  protected setControlValue<K extends keyof TModel>(
    controlName: K,
    value: Nullable<TModel[K]>,
  ): void {
    const control = this.formGroup.get(controlName as string);
    if (!control) {
      throw new Error(`The control [${String(controlName)}] does not exist.`);
    }

    if (control instanceof FormArray) {
      throw new Error('FormArray not supported');
    } else {
      control.setValue(value as any);
    }
  }

  protected setLoader(loading: boolean): void {
    this.loading = loading;
    this.cdr.markForCheck();
  }

  protected abstract submitCore(): void;
  protected abstract buildForm(builder: FormBuilder): FormGroupOf<TModel>;
}

export type AutoSizeStrategy =
  | SizeColumnsToFitGridStrategy
  | SizeColumnsToFitProvidedWidthStrategy
  | SizeColumnsToContentStrategy;
