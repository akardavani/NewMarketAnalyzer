import { Component } from '@angular/core';
import { ServerSideFilterGrid } from '@directives/filter-grid';
import { UnknownLoginHistory, UnknownLoginHistoryFilter } from '../membership-models';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ColDef, GetRowIdParams } from 'ag-grid-enterprise';
import { GridImports } from '@imports';
import { UnknownLoginHistoryService } from './unknown-login-history.service';
import { firstCellValueGetter, formatDateTime } from '@grid-utils';
import { ApiService } from '@services/api.service';
import { SelectOption } from '@typings';
import { Observable } from 'rxjs';

@Component({
  selector: 'fd-unknown-login-history',
  imports: [...GridImports],
  providers: [UnknownLoginHistoryService],
  templateUrl: './unknown-login-history.component.html',
  styleUrl: './unknown-login-history.component.scss'
})
export class UnknownLoginHistoryComponent extends ServerSideFilterGrid<UnknownLoginHistory, UnknownLoginHistoryFilter> {

  loginTypes: ReadonlyArray<SelectOption>;
  applications: Observable<ReadonlyArray<SelectOption>>;

  override columnDefs: ColDef<UnknownLoginHistory, any>[] = [
    { headerName: "ردیف", valueGetter: firstCellValueGetter(), sortable: false },
    { headerName: 'نام کاربری', field: 'userName' },
    { headerName: 'زمان', field: 'eventDateTime', valueFormatter: formatDateTime },
    { headerName: 'IP', field: 'ipAddress' },
    { headerName: 'نوع ورود', field: 'loginType' },
    { headerName: 'برنامه', field: 'application' },
    { headerName: 'مرورگر', field: 'userAgent' },
  ];

  constructor(service: UnknownLoginHistoryService, apiService: ApiService) {
    super(service);

    this.loginTypes = apiService.loginTypes;
    this.applications = apiService.applications;
  }

  override getRowId(params: GetRowIdParams<UnknownLoginHistory>): string {
    return `${params.data.userName}:${params.data.ipAddress}:${params.data.eventDateTime}:${params.data.userAgent}`;
  }

  protected override buildFilter(builder: FormBuilder): FormGroup {
    return builder.group({
      fromDate: [null],
      toDate: [null],
      userName: [null],
      loginType: [null],
      ipAddress: [null],
      userAgent: [null],
      applicationId: [null]
    });
  }

}
