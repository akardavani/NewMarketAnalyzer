import { Injector } from '@angular/core';
import { GridBase } from '@directives/grid-base';
import { Func, SelectOption } from '@typings';
import {
  formatDateTime as DateTimeFormatter,
  formatCurrency as CurrencyFormatter,
  formatNegative as NegativeFormatter,
  parseDateTime,
} from '@utils';
import {
  CellClassFunc,
  CellClassParams,
  ICellRendererParams,
  IRowNode,
  ValueFormatterParams,
  ValueGetterFunc,
} from 'ag-grid-enterprise';
import AutoNumeric from 'autonumeric';
import { Observable } from 'rxjs';

export type IdOwner = { id: any };
export function isIdOwner(obj: any): obj is IdOwner {
  return obj && 'id' in obj;
}

//#region [ValueFormatters]

export function formatDateTime<TData = any>(
  params: ValueFormatterParams<TData, string>,
): string {
  return DateTimeFormatter(parseDateTime(params.value), 'yyyy/MM/dd HH:mm:ss');
}

export function formatDate<TData = any>(
  params: ValueFormatterParams<TData, string>,
): string {
  return DateTimeFormatter(parseDateTime(params.value), 'yyyy/MM/dd');
}

export function formatTime<TData = any>(
  params: ValueFormatterParams<TData, string>,
): string {
  return DateTimeFormatter(parseDateTime(params.value), 'HH:mm:ss');
}

//export function formatCurrency<TData = any>(params: ValueFormatterParams<TData, number>): string {
//  return (params.value == null) ?
//    '' :
//    CurrencyFormatter(params.value);
//}

export function formatCurrency<TData = any>(
  params: ValueFormatterParams<TData, number>,
): string {
  return params.value == null
    ? ''
    : CurrencyFormatter(Math.round(params.value));
}

export function formatNegative<TData = any>(
  params: ValueFormatterParams<TData, number>,
): string {
  return params.value == null ? '' : NegativeFormatter(params.value);
}

export function formatCurrencyAbs<TData = any>(
  params: ValueFormatterParams<TData, number>,
): string {
  return params.value == null
    ? ''
    : CurrencyFormatter(Math.abs(Math.round(params.value)));
}

//#endregion

//#region [ValueGetters]

export function firstCellValueGetter<TData = any>(
  title: string = 'جمع کل',
): ValueGetterFunc<TData, number | string> {
  return (params) => {
    const rowIndex = params.node!.rowIndex!;
    return !params.node!.rowPinned ? rowIndex + 1 : title;
  };
}

//#endregion

//#region [CellClass]

export function ltrCss<TData, TValue>(
  params: CellClassParams<TData, TValue>,
): string | string[] | null | undefined {
  return Number(params.value) < 0 ? 'ltr' : undefined;
}

export function negativeCss<TData, TValue>(
  params: CellClassParams<TData, TValue>,
): string | string[] | null | undefined {
  const classNames: string[] = [];

  if (Number(params.value) < 0) {
    classNames.push('negative');
  }
  if (params.node!.rowPinned) {
    classNames.push('fw-bold');
  }

  return classNames;
}

export function boldPinnedCss<TData, TValue>(
  params: CellClassParams<TData, TValue>,
): string | string[] | null | undefined {
  return params.node!.rowPinned ? 'fw-bold' : null;
}

export function confirmedCss<
  TData = any,
  TValue = any,
  K extends keyof TData = any,
>(property: K): CellClassFunc<TData, TValue> {
  return (params) => {
    const value = params.data![property];
    return value === true
      ? 'text-success'
      : value === false
        ? 'text-danger'
        : undefined;
  };
}

//#endregion

export function faCompare(
  valueA: string | null | undefined,
  valueB: string | null | undefined,
): number {
  return (valueA || '').localeCompare(valueB || '', 'fa');
}

export function defineActionCellParams<
  TData = any,
  TValue = any,
  TContext = any,
>(
  factory: ActionCellFactory<TData, TValue, TContext>,
): { factory: ActionCellFactory<TData, TValue, TContext> } {
  return { factory: factory };
}

//#region [Types]

export interface IActionCellRendererParams<
  TData = any,
  TValue = any,
  TContext = any,
> extends ICellRendererParams<TData, TValue, TContext> {
  factory: ActionCellFactory<TData, TValue, TContext>;
}

export interface IDropdownCellRendererParams<
  TData = any,
  TValue = any,
  TContext = any,
> extends ICellRendererParams<TData, TValue, TContext> {
  caption?: string;
  captionValue?: any;
  options:
    | Observable<ReadonlyArray<SelectOption<TValue>>>
    | DropDownOptionsFunc<TValue>;
  onChanged?: (value: any, data: TData) => void;
}

type DropDownOptionsFunc<TKey = number> = () => Observable<
  ReadonlyArray<SelectOption<TKey>>
>;

export interface IActionCommand<TData = any> {
  action: (data: TData) => any;
  icon?: string;
  tooltip?: string;
  label?: string;
  cssClass?: string;
}

export interface ILinkCellRendererParams<
  TData = any,
  TValue = any,
  TContext = any,
> extends ICellRendererParams<TData, TValue, TContext> {
  target?: string;
  icon?: string;
  href: string | LinkItemFactory<TData, TValue, TContext>;
}

export interface LinkItemFactory<TData = any, TValue = any, TContext = any> {
  (params: ICellRendererParams<TData, TValue, TContext>): any;
}

export interface ActionCellFactory<TData = any, TValue = any, TContext = any> {
  (
    params: ICellRendererParams<TData, TValue, TContext>,
  ): IActionCommand<TData>[];
}

export interface InputCellRendererParams<
  TData = any,
  TValue = any,
> extends ICellRendererParams<TData, TValue> {
  type?: string;
  autoNumeric?: AutoNumeric.Options;
  onChange?: (value: any) => void;
}

//#endregion
