import { Component } from '@angular/core';
import { CrudGrid } from '@directives/crud-grid';
import { ProspectusRequest, ProspectusRequestFilter } from '../../cms-models';
import { apiUrl, bind } from '@utils';
import { CreateProspectusComponent } from '../create-prospectus/create-prospectus.component';
import { ColDef, ICellRendererParams } from 'ag-grid-enterprise';
import {
  defineActionCellParams,
  firstCellValueGetter,
  formatDate,
  IActionCommand,
} from '@grid-utils';
import { ActionCellRendererComponent } from '@components/action-cell-renderer/action-cell-renderer.component';
import { ReportService } from '@services/report.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ReportFormat } from '@typings';
import { ProspectusService } from './prospectus.service';
import { GridImports } from '@imports';

@Component({
  selector: 'fd-prospectus',
  imports: [...GridImports],
  providers: [ProspectusService],
  templateUrl: './prospectus.component.html',
  styleUrl: './prospectus.component.scss',
})
export class ProspectusComponent extends CrudGrid<
  ProspectusRequest,
  ProspectusRequestFilter
> {
  protected deleteUrl = apiUrl('Prospectus/Delete');
  protected createComponent = CreateProspectusComponent;
  protected editComponent = CreateProspectusComponent;

  override columnDefs: ColDef<ProspectusRequest, any>[] = [
    {
      headerName: 'ردیف',
      valueGetter: firstCellValueGetter(),
      sortable: false,
    },
    { headerName: 'توضیحات', field: 'description' },
    { headerName: 'تاریخ', field: 'date', valueFormatter: formatDate },
    {
      headerName: 'عملیات',
      sortable: false,
      cellRenderer: ActionCellRendererComponent,
      cellRendererParams: defineActionCellParams(this.createActionCommands),
    },
  ];
  constructor(
    service: ProspectusService,
    private readonly reportService: ReportService,
  ) {
    super(service);
  }

  createProspectus() {
    this.dialog.open(CreateProspectusComponent);
  }

  protected buildFilter(builder: FormBuilder): FormGroup {
    return builder.group({
      fromDate: [null],
      toDate: [null],
    });
  }

  @bind()
  private createActionCommands(
    params: ICellRendererParams<ProspectusRequest>,
  ): IActionCommand[] {
    const actions: IActionCommand[] = [
      {
        icon: 'pa pa-delete',
        tooltip: 'حذف',
        action: this.delete,
      },
      {
        icon: 'pa pa-download',
        tooltip: 'دانلود امیدنامه',
        action: () =>
          this.reportService
            .export(ReportFormat.Pdf, apiUrl('Prospectus/ExportProspectus'), {
              attachmentId: params.data?.attachmentId,
            })
            .subscribe(() => {}),
      },
    ];
    return actions;
  }
}
