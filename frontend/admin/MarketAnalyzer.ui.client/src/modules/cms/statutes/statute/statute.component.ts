import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ServerSideFilterGrid } from '@directives/filter-grid';
import { GridImports } from '@imports';
import { StatuteService } from './statute.service';
import { ColDef, ICellRendererParams } from 'ag-grid-enterprise';
import {
  defineActionCellParams,
  firstCellValueGetter,
  formatDate,
  IActionCommand,
} from '@grid-utils';
import { ActionCellRendererComponent } from '@components/action-cell-renderer/action-cell-renderer.component';
import { apiUrl, bind } from '@utils';
import { CrudGrid, CrudHostGrid } from '@directives/crud-grid';
import { ReportService } from '@services/report.service';
import { ApiResult, ReportFormat } from '@typings';
import { StatuteRequest, StatuteRequestFilter } from '../../cms-models';
import { DialogService } from '@services/dialog.service';
import { CreateStatuteComponent } from '../create-statute/create-statute.component';

@Component({
  selector: 'fd-statute',
  imports: [...GridImports],
  providers: [StatuteService],
  templateUrl: './statute.component.html',
  styleUrl: './statute.component.scss',
})
export class StatuteComponent extends CrudGrid<
  StatuteRequest,
  StatuteRequestFilter
> {
  protected deleteUrl = apiUrl('Statute/Delete');
  protected createComponent = CreateStatuteComponent;
  protected editComponent = CreateStatuteComponent;

  override columnDefs: ColDef<StatuteRequest, any>[] = [
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
    service: StatuteService,
    private readonly reportService: ReportService,
  ) {
    super(service);
  }

  createStatute() {
    this.dialog.open(CreateStatuteComponent);
  }

  protected buildFilter(builder: FormBuilder): FormGroup {
    return builder.group({
      fromDate: [null],
      toDate: [null],
    });
  }

  @bind()
  private createActionCommands(
    params: ICellRendererParams<StatuteRequest>,
  ): IActionCommand[] {
    const actions: IActionCommand[] = [
      {
        icon: 'pa pa-delete',
        tooltip: 'حذف',
        action: this.delete,
      },
      {
        icon: 'pa pa-download',
        tooltip: 'دانلود اساسنامه',
        action: () =>
          this.reportService
            .export(ReportFormat.Pdf, apiUrl('Statute/ExportStatutes'), {
              attachmentId: params.data?.attachmentId,
            })
            .subscribe(() => {}),
      },
    ];
    return actions;
  }
}
