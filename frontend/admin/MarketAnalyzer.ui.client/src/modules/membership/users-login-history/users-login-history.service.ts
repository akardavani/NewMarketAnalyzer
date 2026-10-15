import { Injectable } from '@angular/core';
import { ServerSideDataSourceService } from '@services/data-source.service';
import { UsersLoginHistory, UsersLoginHistoryFilter } from '../membership-models';
import { apiUrl } from '@utils';

@Injectable()
export class UsersLoginHistoryService extends ServerSideDataSourceService<UsersLoginHistory, UsersLoginHistoryFilter> {
  protected override url: string = apiUrl('Account/UsersLoginHistory');
}
