import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StrategyPageBase } from '../../shared/strategy-page.base';
import { daysAgo, today } from '../../shared/jalali';
import { LineChartComponent, ChartSeriesInput } from '../../shared/line-chart/line-chart.component';
import { SymbolPickerComponent } from '../../shared/symbol-picker/symbol-picker.component';
import { SymbolItem } from '../../shared/strategy-api.service';

@Component({
  selector: 'mi-ratio-candles',
  imports: [FormsModule, LineChartComponent, SymbolPickerComponent],
  templateUrl: './ratio-candles.component.html',
  styleUrl: './ratio-candles.component.scss'
})
export class RatioCandlesComponent extends StrategyPageBase {
  readonly strategyName = 'RatioCandles';
  symbols: SymbolItem[] = [];
  startDate = daysAgo(30);
  endDate = today();
  timeframe = 'Daily';

  protected buildParams(p: string[]) {
    if (!this.symbols.length) p.push('حداقل یک نماد انتخاب کنید.');
    const s = this.isoDate('تاریخ شروع', this.startDate, p);
    const e = this.isoDate('تاریخ پایان', this.endDate, p);
    return p.length ? null : { Symbols: this.symbols.map(x => x.symbolKey), StartDate: s, EndDate: e, Timeframe: this.timeframe };
  }

  private label(key: string): string {
    const s = this.symbols.find(x => x.symbolKey === key || x.symbol === key);
    return s ? `${s.name || s.symbol} (${s.symbolKey})` : key;
  }

  get series(): ChartSeriesInput[] {
    const d = this.data();
    if (!Array.isArray(d)) return [];
    return d.map((x: any) => ({
      label: this.label(x.symbol ?? x.instrumentId),
      points: (x.results ?? []).map((r: any) => ({ label: r.persianDate, value: Number(r.growth ?? 0) }))
    }));
  }
  isEmpty(): boolean { return !this.series.some(s => s.points.length); }
  status(): string { return ''; }
}
