import { Component, Input } from '@angular/core';
import { ILoadingOverlayAngularComp } from 'ag-grid-angular';
import { ILoadingOverlayParams } from 'ag-grid-enterprise';

@Component({
  selector: 'fd-loader',
  imports: [],
  templateUrl: './loader.component.html',
  styleUrl: './loader.component.scss'
})
export class LoaderComponent implements ILoadingOverlayAngularComp {

  @Input()
  message = 'لطفا منتظر باشید...';

  agInit(params: ILoadingOverlayParams<any, any>): void {

  }

  //refresh?(params: ILoadingOverlayParams<any, any>): void {
  //    throw new Error('Method not implemented.');
  //}

}
