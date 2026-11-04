import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StrategyPageBase } from '../../shared/strategy-page.base';
import { daysAgo, today } from '../../shared/jalali';
import { DataTableComponent } from '../../shared/data-table/data-table.component';
import { RunBottomReversalRequest } from '../../shared/strategy-api.service';

@Component({
  selector: 'mi-bottom-reversal',
  imports: [FormsModule, DataTableComponent],
  templateUrl: './bottom-reversal.component.html',
  styleUrl: './bottom-reversal.component.scss'
})
export class BottomReversalComponent extends StrategyPageBase {
  readonly strategyName = 'BottomReversal';
  inputFilePath = '';
  startDate = daysAgo(30);
  endDate = today();

  protected buildParams(p: string[]) {
    this.required('مسیر فایل اکسل', this.inputFilePath, p);
    const s = this.isoDate('تاریخ شروع', this.startDate, p);
    const e = this.isoDate('تاریخ پایان', this.endDate, p);
    return p.length ? null : { InputFilePath: this.inputFilePath.trim(), StartDate: s, EndDate: e };
  }

  override run(): void {
    this.resetState();
    const problems: string[] = [];
    const params = this.buildParams(problems);
    if (problems.length || !params) { this.errors.set(problems); return; }

    const request: RunBottomReversalRequest = {
      inputFilePath: String(params['InputFilePath']),
      startDate: String(params['StartDate']),
      endDate: String(params['EndDate'])
    };
    this.execute(this.api.runBottomReversal(request));
  }

  readonly pickCols = [
    { key: 'symbol', label: 'نماد' }, { key: 'tier', label: 'سطح' }, { key: 'finalScore', label: 'امتیاز' },
    { key: 'percentile', label: 'صدک' }, { key: 'lastPrice', label: 'آخرین قیمت' }, { key: 'entry', label: 'ورود' },
    { key: 'stopLoss', label: 'حد ضرر' }, { key: 'takeProfits', label: 'اهداف' }, { key: 'fairValue', label: 'ارزش ذاتی' },
    { key: 'distanceToSupport', label: 'فاصله تا حمایت' }, { key: 'distanceToResistance', label: 'فاصله تا مقاومت' },
    { key: 'rawOversold', label: 'اشباع فروش' }, { key: 'rawProximity', label: 'نزدیکی به کف' },
    { key: 'rawReversalStrength', label: 'قدرت برگشت' }, { key: 'rawBottomQuality', label: 'کیفیت کف' }, { key: 'reasons', label: 'دلایل' }
  ];
  readonly sectorCols = [
    { key: 'sectorName', label: 'صنعت' }, { key: 'averageScore', label: 'میانگین امتیاز' },
    { key: 'momentumScore', label: 'قدرت روند' }, { key: 'totalMoneyFlow', label: 'ورود پول' }
  ];
  status(): string { return this.data()?.status ?? ''; }
  isEmpty(): boolean { return !this.data()?.topPicks?.length && !this.data()?.fullSectorReport?.length; }
}
