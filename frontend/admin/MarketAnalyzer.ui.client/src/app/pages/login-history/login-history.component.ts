import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ServerSideFilterGrid } from '@directives/filter-grid';
import { firstCellValueGetter, formatDateTime } from '@grid-utils';
import { GridImports } from '@imports';
import { ApiService } from '@services/api.service';
import { ServerSideDataSourceService } from '@services/data-source.service';
import { SelectOption } from '@typings';
import { apiUrl } from '@utils';
import { ColDef } from 'ag-grid-enterprise';
import { Observable } from 'rxjs';

class LoginHistoryService extends ServerSideDataSourceService<LoginHistory, LoginHistoryFilter> {
  protected override url: string = apiUrl('Account/UserLoginHistory');
}


@Component({
  selector: 'fd-login-history',
  imports: [...GridImports],
  providers: [LoginHistoryService],
  templateUrl: './login-history.component.html',
  styleUrl: './login-history.component.scss'
})
export class LoginHistoryComponent extends ServerSideFilterGrid<LoginHistory, LoginHistoryFilter> {

  override columnDefs: ColDef<LoginHistory, any>[] = [
    { headerName: "ردیف", valueGetter: firstCellValueGetter(), sortable: false },
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

  constructor(service: LoginHistoryService, apiService: ApiService) {
    super(service);

    this.loginTypes = apiService.loginTypes;
    this.applications = apiService.applications;
    this.loginResults = apiService.loginResults;
  }

  protected override buildFilter(builder: FormBuilder): FormGroup {
    return builder.group({
      fromDate: [null],
      toDate: [null],
      loginType: [null],
      ipAddress: [null],
      userAgent: [null],
      applicationId: [null],
      resultId: [null]
    });
  }

}

interface LoginHistory {
  id: number;
  loginType: string;
  ipAddress: string;
  userAgent: string;
  loginResult: string;
  application: string;
  eventDateTime: Date;
}

interface LoginHistoryFilter {

}
