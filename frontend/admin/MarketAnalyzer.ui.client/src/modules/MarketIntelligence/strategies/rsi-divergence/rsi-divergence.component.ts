import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StrategyPageBase } from '../../shared/strategy-page.base';
import { DataTableComponent } from '../../shared/data-table/data-table.component';
import { RunRsiDivergenceRequest } from '../../shared/strategy-api.service';

@Component({
  selector: 'mi-rsi-divergence',
  imports: [FormsModule, DataTableComponent],
  templateUrl: './rsi-divergence.component.html',
  styleUrl: './rsi-divergence.component.scss'
})
export class RsiDivergenceComponent extends StrategyPageBase {
  readonly strategyName = 'RsiDivergence';
  includeSuspects = true;
  includeBacktest = true;

  protected buildParams(_: string[]) {
    return { IncludeSuspects: this.includeSuspects, IncludeBacktest: this.includeBacktest };
  }

  override run(): void {
    this.resetState();
    const request: RunRsiDivergenceRequest = {
      includeSuspects: this.includeSuspects,
      includeBacktest: this.includeBacktest
    };
    this.execute(this.api.runRsiDivergence(request));
  }

  readonly signalCols = [
    { key: 'symbol', label: 'نماد' }, { key: 'timeframe', label: 'تایم‌فریم' }, { key: 'status', label: 'وضعیت' },
    { key: 'firstLowDate', label: 'تاریخ کف اول' }, { key: 'secondLowDate', label: 'تاریخ کف دوم' },
    { key: 'firstLow', label: 'قیمت کف اول' }, { key: 'secondLow', label: 'قیمت کف دوم' },
    { key: 'firstRsi', label: 'RSI کف اول' }, { key: 'secondRsi', label: 'RSI کف دوم' },
    { key: 'entryPrice', label: 'ورود تقریبی' }, { key: 'stopPrice', label: 'حد ضرر' },
    { key: 'targetOne', label: 'هدف اول' }, { key: 'targetTwo', label: 'هدف دوم' }, { key: 'currentReturnPercent', label: 'بازده فعلی٪' }
  ];
  readonly suspectCols = [
    { key: 'symbol', label: 'نماد' }, { key: 'timeframe', label: 'تایم‌فریم' }, { key: 'kind', label: 'نوع مورد' },
    { key: 'firstLowDate', label: 'تاریخ کف قبلی' }, { key: 'candidateLowDate', label: 'تاریخ کف احتمالی' },
    { key: 'firstRsi', label: 'RSI کف قبلی' }, { key: 'candidateRsi', label: 'RSI کف احتمالی' },
    { key: 'barsUntilPivotConfirmation', label: 'کندل تا تأیید کف' }
  ];
  readonly backtestCols = [
    { key: 'timeframe', label: 'تایم‌فریم' }, { key: 'trades', label: 'تعداد معاملات' },
    { key: 'winRatePercent', label: 'برد٪' }, { key: 'averageReturnPercent', label: 'میانگین بازده٪' },
    { key: 'medianReturnPercent', label: 'میانه بازده٪' }, { key: 'profitFactor', label: 'Profit factor' },
    { key: 'targetOneHitPercent', label: 'رسیدن به هدف اول٪' }, { key: 'targetTwoHitPercent', label: 'رسیدن به هدف دوم٪' },
    { key: 'stopHitPercent', label: 'حد ضرر٪' }, { key: 'averageBarsHeld', label: 'میانگین کندل نگهداری' }
  ];

  isEmpty(): boolean {
    const result = this.data() as any;
    return !!result && !result.signals?.length && !result.suspects?.length && !result.backtest?.length;
  }
}
