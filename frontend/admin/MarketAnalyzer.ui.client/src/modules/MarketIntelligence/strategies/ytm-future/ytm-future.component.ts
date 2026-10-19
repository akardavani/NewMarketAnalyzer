import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StrategyPageBase } from '../../shared/strategy-page.base';
import { daysAgo, today } from '../../shared/jalali';
import { LineChartComponent, ChartSeriesInput } from '../../shared/line-chart/line-chart.component';
import { DataTableComponent } from '../../shared/data-table/data-table.component';

@Component({
  selector: 'mi-ytm-future',
  imports: [FormsModule, LineChartComponent, DataTableComponent],
  templateUrl: './ytm-future.component.html',
  styleUrl: './ytm-future.component.scss'
})
export class YtmFutureComponent extends StrategyPageBase {
  readonly strategyName = 'YtmFuture';
  startDate = daysAgo(30);
  endDate = today();
  symbol = 'ETC';
  calculates = 'NearestYtm';
  readonly symbolOptions = [{ text: 'طلای لوتوس', value: 'ETC' }, { text: 'سپرده نقره', value: 'SIL' }];
  readonly calcOptions = [
    { text: 'سر رسید نزدیک YTM', value: 'NearestYtm' }, { text: 'سر رسید دوم YTM', value: 'SecondYtm' },
    { text: 'سر رسید دور YTM', value: 'LatestYtm' }, { text: 'سر رسید نزدیک', value: 'NearestContract' },
    { text: 'سر رسید دوم', value: 'SecondContract' }, { text: 'سر رسید دور', value: 'LatestContract' }, { text: 'سود', value: 'Sood' }
  ];

  protected buildParams(p: string[]) {
    const s = this.isoDate('تاریخ شروع', this.startDate, p);
    const e = this.isoDate('تاریخ پایان', this.endDate, p);
    return p.length ? null : { StartDate: s, EndDate: e, Symbol: this.symbol, Calculates: this.calculates };
  }

  get rows(): any[] { return Array.isArray(this.data()) ? (this.data() as any[]) : []; }
  get series(): ChartSeriesInput[] {
    const label = this.calcOptions.find(o => o.value === this.calculates)?.text ?? this.calculates;
    return [{ label, points: this.rows.map(r => ({ label: r.persianDate, value: Number(r.growth ?? 0) })) }];
  }
  readonly cols = [{ key: 'persianDate', label: 'تاریخ' }, { key: 'growth', label: 'مقدار' }];
  isEmpty(): boolean { return !this.rows.length; }
  status(): string { return ''; }
}
