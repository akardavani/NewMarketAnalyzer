import { Injectable } from '@angular/core';
import { ServerSideDataSourceService } from '@services/data-source.service';
import { apiUrl } from '@utils';
import {
  InvestmentPolicyStatementRequest,
  InvestmentPolicyStatementRequestFilter,
} from '../../cms-models';

@Injectable()
export class InvestmentPolicyStatementService extends ServerSideDataSourceService<
  InvestmentPolicyStatementRequest,
  InvestmentPolicyStatementRequestFilter
> {
  protected override url: string = apiUrl('InvestmentPolicyStatement/GetAll');
}
