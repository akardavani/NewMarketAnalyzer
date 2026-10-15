import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-enterprise';

@Component({
  selector: 'fd-checked-cell-renderer',
  imports: [],
  templateUrl: './checked-cell-renderer.component.html',
  styleUrl: './checked-cell-renderer.component.scss'
})
export class CheckedCellRendererComponent implements ICellRendererAngularComp {

  value = false;

  agInit(params: ICellRendererParams<any, any, any>): void {
    this.value = params.value;
  }

  refresh(params: ICellRendererParams<any, any, any>): boolean {
    this.value = params.value;
    return true;
  }

}
