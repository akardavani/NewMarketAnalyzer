import { Injectable } from '@angular/core';
import { ServerSideDataSourceService } from '@services/data-source.service';
import { Users, UsersFilter } from '../../membership-models';
import { apiUrl } from '@utils';

@Injectable()
export class UsersService extends ServerSideDataSourceService<Users, UsersFilter> {
  protected override url: string = apiUrl('User/GetAll');
}
