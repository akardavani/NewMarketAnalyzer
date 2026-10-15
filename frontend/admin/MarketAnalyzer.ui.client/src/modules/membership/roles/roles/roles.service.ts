import { Injectable } from '@angular/core';
import { ServerSideDataSourceService } from '@services/data-source.service';
import { Roles, RolesFilter } from '../../membership-models';
import { apiUrl } from '@utils';

@Injectable()
export class RolesService extends ServerSideDataSourceService<Roles, RolesFilter> {
  protected override url: string = apiUrl('Role/GetAll');
}
