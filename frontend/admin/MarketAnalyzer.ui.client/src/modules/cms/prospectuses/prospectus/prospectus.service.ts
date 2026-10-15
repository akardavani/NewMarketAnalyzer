import { Injectable } from '@angular/core';
import { ServerSideDataSourceService } from '@services/data-source.service';
import { apiUrl } from '@utils';
import { ProspectusRequest, ProspectusRequestFilter } from '../../cms-models';

@Injectable()
export class ProspectusService extends ServerSideDataSourceService<
  ProspectusRequest,
  ProspectusRequestFilter
> {
  protected override url: string = apiUrl('Prospectus/GetAll');
}
