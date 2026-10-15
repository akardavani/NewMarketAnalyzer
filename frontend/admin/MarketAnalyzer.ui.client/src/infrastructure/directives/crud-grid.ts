import { Directive, inject, Injector, Type } from "@angular/core";
import { ServerSideDataSourceService } from "@services/data-source.service";
import { GetRowIdParams, ICellRendererParams } from "ag-grid-enterprise";
import { ServerSideFilterGrid } from "./filter-grid";
import { ApiResult } from "@typings";
import { ModalService } from "@services/modal.service";
import { MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { bind } from "@utils";
import { DialogService } from "@services/dialog.service";

/** کامپوننت هایی که حداقل یکی از عملیات افزودن یا ویرایش را پشتیبانی میکنند */
@Directive()
export abstract class CrudHostGrid<TData = void, TFilter = void> extends ServerSideFilterGrid<TData, TFilter> {

  protected readonly dialog: DialogService;

  constructor(dataSource: ServerSideDataSourceService<TData, TFilter>) {
    super(dataSource);
    this.dialog = inject(DialogService);
  }

  protected openDialog<T>(component: Type<T>, configure?: (confg: MatDialogConfig) => void): MatDialogRef<T, any> {
    return this.dialog.open(component, this, configure);
  }

}

@Directive()
export abstract class CrudGrid<TData = void, TFilter = void> extends CrudHostGrid<TData, TFilter> {

  protected readonly modal: ModalService;

  protected abstract deleteUrl: string;
  protected abstract editComponent: Type<any>;
  protected abstract createComponent: Type<any>;

  constructor(dataSource: ServerSideDataSourceService<TData, TFilter>) {
    super(dataSource);

    this.modal = inject(ModalService);
  }

  @bind()
  create(): void {
    this.openDialog(this.createComponent, config => {
      this.configDialog('create', config);
    });
    //dialog.afterClosed().subscribe(() => this.refreshGrid());
  }

  @bind()
  edit<TValue = any, TContext = any>(cellParams: ICellRendererParams<TData, TValue, TContext>): void {
    this.dialog.open(this.editComponent, config => {
      config.data = this.getRequestParams('edit', cellParams);
      this.configDialog('edit', config);
    });
    //dialog.afterClosed().subscribe(() => this.refreshGrid());
  }

  @bind()
  delete<TValue = any, TContext = any>(cellParams: ICellRendererParams<TData, TValue, TContext>): void {
    this.modal.open({
      title: 'حذف رکورد',
      content: 'آیا از حذف رکورد اطمینان دارید؟',
      onConfirm: (result) => {
        if (result == 'ok') {
          const data = this.getRequestParams('delete', cellParams);
          this.httpClient
            .post<ApiResult>(this.deleteUrl, data)
            .subscribe(response => {
              if (response.successed) {
                this.refresh();
              }
            });
        }

        return true;
      }
    });
  }

  protected configDialog(type: GridRequestType, config: MatDialogConfig): void {
    if (!config) {
      throw new Error('config is required.');
    }
  }

  protected getRequestParams<TValue = any, TContext = any>(type: GridRequestType, params: ICellRendererParams<TData, TValue, TContext>): any {
    if (type != 'edit' && type != 'delete') {
      throw new Error(`Not supported type: '${type}'.`);
    }

    return {
      id: this.getRowId(this.mapGetRowIdParams(params))
    };
  }

  protected mapGetRowIdParams<TValue = any, TContext = any>(params: ICellRendererParams<TData, TValue, TContext>): GetRowIdParams<TData, TContext> {
    return {
      data: params.data!,
      api: params.api,
      context: params.context,
      level: params.node.level,
      rowPinned: params.node.rowPinned
    };
  }

}

export type GridRequestType = 'create' | 'edit' | 'delete';
