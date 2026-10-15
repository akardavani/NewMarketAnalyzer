import { Component } from '@angular/core';
import { InvestmentPolicyStatementService } from './investment-policy-statement.service';
import { GridImports } from '@imports';
import { CreateInvestmentPolicyStatementComponent } from '../create-investment-policy-statement/create-investment-policy-statement.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActionCellRendererComponent } from '@components/action-cell-renderer/action-cell-renderer.component';
import { CrudGrid } from '@directives/crud-grid';
import {
  firstCellValueGetter,
  formatDate,
  defineActionCellParams,
  IActionCommand,
} from '@grid-utils';
import { ReportService } from '@services/report.service';
import { ReportFormat } from '@typings';
import { apiUrl, bind } from '@utils';
import { ColDef, ICellRendererParams } from 'ag-grid-enterprise';
import {
  InvestmentPolicyStatementRequest,
  InvestmentPolicyStatementRequestFilter,
  StatuteRequest,
} from '../../cms-models';
import { CheckedCellRendererComponent } from '@components/checked-cell-renderer/checked-cell-renderer.component';

@Component({
  selector: 'fd-investment-policy-statement',
  imports: [...GridImports],
  providers: [InvestmentPolicyStatementService],
  templateUrl: './investment-policy-statement.component.html',
  styleUrl: './investment-policy-statement.component.scss',
})
export class InvestmentPolicyStatementComponent extends CrudGrid<
  InvestmentPolicyStatementRequest,
  InvestmentPolicyStatementRequestFilter
> {
  protected deleteUrl = apiUrl('InvestmentPolicyStatement/Delete');
  protected createComponent = CreateInvestmentPolicyStatementComponent;
  protected editComponent = CreateInvestmentPolicyStatementComponent;

  override columnDefs: ColDef<InvestmentPolicyStatementRequest, any>[] = [
    {
      headerName: 'ردیف',
      valueGetter: firstCellValueGetter(),
      sortable: false,
    },
    { headerName: 'توضیحات', field: 'description' },
    { headerName: 'تاریخ', field: 'date', valueFormatter: formatDate },
    {
      headerName: 'توضیحات',
      field: 'isActive',
      cellRenderer: CheckedCellRendererComponent,
    },
    {
      headerName: 'عملیات',
      sortable: false,
      cellRenderer: ActionCellRendererComponent,
      cellRendererParams: defineActionCellParams(this.createActionCommands),
    },
  ];
  constructor(
    service: InvestmentPolicyStatementService,
    private readonly reportService: ReportService,
  ) {
    super(service);
  }

  createStatute() {
    this.dialog.open(CreateInvestmentPolicyStatementComponent);
  }

  protected buildFilter(builder: FormBuilder): FormGroup {
    return builder.group({
      fromDate: [null],
      toDate: [null],
    });
  }

  @bind()
  private createActionCommands(
    params: ICellRendererParams<InvestmentPolicyStatementRequest>,
  ): IActionCommand[] {
    const actions: IActionCommand[] = [
      {
        icon: 'pa pa-delete',
        tooltip: 'حذف',
        action: this.delete,
      },
      {
        icon: 'pa pa-download',
        tooltip: 'دانلود بیانیه سیاست های سرمایه گذاری',
        action: () =>
          this.reportService
            .export(
              ReportFormat.Pdf,
              apiUrl('InvestmentPolicyStatement/ExportInvestmentPolicy'),
              {
                attachmentId: params.data?.attachmentId,
              },
            )
            .subscribe(() => {}),
      },
    ];
    return actions;
  }
}
