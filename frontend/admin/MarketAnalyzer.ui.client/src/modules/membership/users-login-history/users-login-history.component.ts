import { Component } from '@angular/core';
import { UsersLoginHistory, UsersLoginHistoryFilter } from '../membership-models';
import { ServerSideFilterGrid } from '@directives/filter-grid';
import { UsersLoginHistoryService } from './users-login-history.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ColDef } from 'ag-grid-enterprise';
import { firstCellValueGetter, formatDateTime } from '@grid-utils';
import { ApiService } from '@services/api.service';
import { SelectOption } from '@typings';
import { GridImports } from '@imports';
import { Observable } from 'rxjs';

@Component({
  selector: 'fd-users-login-history',
  imports: [...GridImports],
  providers: [UsersLoginHistoryService],
  templateUrl: './users-login-history.component.html',
  styleUrl: './users-login-history.component.scss'
})
export class UsersLoginHistoryComponent extends ServerSideFilterGrid<UsersLoginHistory, UsersLoginHistoryFilter> {

  override columnDefs: ColDef<UsersLoginHistory, any>[] = [
    { headerName: "ردیف", valueGetter: firstCellValueGetter(), sortable: false },
    { headerName: 'نام کاربری', field: 'userName' },
    { headerName: 'نام', field: 'fullName' },
    { headerName: 'زمان', field: 'eventDateTime', valueFormatter: formatDateTime },
    { headerName: 'نوع ورود', field: 'loginType' },
    { headerName: 'وضعیت', field: 'loginResult' },
    { headerName: 'IP', field: 'ipAddress' },
    { headerName: 'برنامه', field: 'application' },
    { headerName: 'مرورگر', field: 'userAgent' },
  ];

  loginTypes: ReadonlyArray<SelectOption>;
  applications: Observable<ReadonlyArray<SelectOption>>;
  loginResults: Observable<ReadonlyArray<SelectOption>>;

  constructor(service: UsersLoginHistoryService, apiService: ApiService) {
    super(service);

    this.loginTypes = apiService.loginTypes;
    this.applications = apiService.applications;
    this.loginResults = apiService.loginResults;
  }

  protected override buildFilter(builder: FormBuilder): FormGroup {
    return builder.group({
      fromDate: [null],
      toDate: [null],
      userName: [null],
      loginType: [null],
      ipAddress: [null],
      userAgent: [null],
      applicationId: [null],
      resultId: [null]
    });
  }

}
