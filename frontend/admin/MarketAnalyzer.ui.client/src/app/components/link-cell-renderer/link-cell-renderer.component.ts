import { Component } from '@angular/core';
import { ILinkCellRendererParams } from '@grid-utils';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-enterprise';
import { NgClass } from '../../../../node_modules/@angular/common/types/_common_module-chunk';

@Component({
  selector: 'fd-link-cell-renderer',
  templateUrl: './link-cell-renderer.component.html',
  styleUrl: './link-cell-renderer.component.scss',
})
export class LinkCellRendererComponent implements ICellRendererAngularComp {
  value: any;
  target?: string;
  href: string = '#';

  agInit(params: ICellRendererParams<any, any, any>): void {
    this.init(params as ILinkCellRendererParams);
  }

  refresh(params: ICellRendererParams<any, any, any>): boolean {
    this.init(params as ILinkCellRendererParams);
    return true;
  }

  private init(params: ILinkCellRendererParams): void {
    this.target = params.target || '_blank';
    this.value = params.valueFormatted || params.value;
    this.href =
      typeof params.href === 'string' ? params.href : params.href(params);
  }
}
