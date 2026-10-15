import { Routes } from '@angular/router';
import { StatuteComponent } from './statutes/statute/statute.component';
import { ProspectusComponent } from './prospectuses/prospectus/prospectus.component';
import { InvestmentPolicyStatementComponent } from './investmentPolicyStatement/investment-policy-statement/investment-policy-statement.component';

export const routes: Routes = [
  { path: 'statutes', title: 'اساسنامه', component: StatuteComponent },
  { path: 'prospectus', title: 'امیدنامه', component: ProspectusComponent },
  {
    path: 'policy-statement',
    title: 'بیانیه سیاست های سرمایه گذاری',
    component: InvestmentPolicyStatementComponent,
  },
];
