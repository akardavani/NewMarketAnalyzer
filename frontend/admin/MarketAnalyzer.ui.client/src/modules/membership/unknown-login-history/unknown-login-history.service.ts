import { Injectable } from '@angular/core';
import { ServerSideDataSourceService } from '@services/data-source.service';
import { UnknownLoginHistory, UnknownLoginHistoryFilter } from '../membership-models';
import { apiUrl } from '@utils';

@Injectable()
export class UnknownLoginHistoryService extends ServerSideDataSourceService<UnknownLoginHistory, UnknownLoginHistoryFilter> {
  protected override url: string = apiUrl('Account/UnknownLoginHistory');
}
