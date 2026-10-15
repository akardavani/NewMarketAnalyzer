import { Injectable } from '@angular/core';
import { ServerSideDataSourceService } from '@services/data-source.service';
import { apiUrl } from '@utils';
import { StatuteRequest, StatuteRequestFilter } from '../../cms-models';

@Injectable()
export class StatuteService extends ServerSideDataSourceService<
  StatuteRequest,
  StatuteRequestFilter
> {
  protected override url: string = apiUrl('Statute/GetAll');
}
