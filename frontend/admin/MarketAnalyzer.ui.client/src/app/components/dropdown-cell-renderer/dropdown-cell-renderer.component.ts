import { AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IDropdownCellRendererParams } from '@grid-utils';
import { SelectOption } from '@typings';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'fd-dropdown-cell-renderer',
  imports: [FormsModule, ReactiveFormsModule, AsyncPipe],
  templateUrl: './dropdown-cell-renderer.component.html',
  styleUrl: './dropdown-cell-renderer.component.scss'
})
export class DropdownCellRendererComponent<TData = any, TValue = any, TContext = any> implements ICellRendererAngularComp {

  value: any;
  captionValue: any;
  caption: string | undefined;
  options: Observable<ReadonlyArray<SelectOption<TValue>>> = of([]);
  private params!: IDropdownCellRendererParams<TData, TValue, TContext>;

  constructor() {

  }

  agInit(params: IDropdownCellRendererParams<TData, TValue, TContext>): void {
    this.init(params);
  }

  refresh(params: IDropdownCellRendererParams<TData, TValue, TContext>): boolean {
    this.init(params);
    return true;
  }

  changed(): void {
    if (this.params.onChanged) {
      this.params.onChanged(this.value, this.params.data!);
    }
  }

  private init(params: IDropdownCellRendererParams<TData, TValue, TContext>): void {
    this.params = params;
    this.value = params.value;
    this.options = typeof params.options === 'function' ? params.options() : params.options;
    this.caption = params.caption;
    this.captionValue = params.captionValue;
  }

}
