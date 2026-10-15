import { Component } from '@angular/core';
import { IActionCellRendererParams, IActionCommand } from '@grid-utils';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-enterprise';

@Component({
  selector: 'fd-action-cell-renderer',
  imports: [],
  templateUrl: './action-cell-renderer.component.html',
  styleUrl: './action-cell-renderer.component.scss',
  host: {
    'class': 'ag-cell-action'
  }
})
export class ActionCellRendererComponent implements ICellRendererAngularComp {

  actions!: IActionCommand<any>[];
  rowParams!: ICellRendererParams<any, any, any>;

  constructor() {

  }

  agInit(params: ICellRendererParams<any, any, any>): void {
    this.init(params as IActionCellRendererParams);
  }

  refresh(params: ICellRendererParams<any, any, any>): boolean {
    this.init(params as IActionCellRendererParams);
    return true;
  }

  private init(params: IActionCellRendererParams): void {
    this.rowParams = params;
    this.actions = params.factory(params);
  }
}
