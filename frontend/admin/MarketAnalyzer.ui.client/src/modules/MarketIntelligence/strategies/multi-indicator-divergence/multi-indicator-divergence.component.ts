import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StrategyPageBase } from '../../shared/strategy-page.base';
import { daysAgo, today } from '../../shared/jalali';
import { DataTableComponent } from '../../shared/data-table/data-table.component';

@Component({
  selector: 'mi-multi-indicator-divergence',
  imports: [FormsModule, DataTableComponent],
  templateUrl: './multi-indicator-divergence.component.html',
  styleUrl: './multi-indicator-divergence.component.scss'
})
export class MultiIndicatorDivergenceComponent extends StrategyPageBase {
  readonly strategyName = 'MultiIndicatorDivergence';
  inputFilePath = '';

  protected buildParams(p: string[]) {
    this.required('مسیر فایل اکسل', this.inputFilePath, p);
    return p.length ? null : { InputFilePath: this.inputFilePath.trim() };
  }

  /** Flatten detectedDivergences into one readable column. */
  get rows(): any[] {
    return (this.data()?.topPicks ?? []).map((x: any) => ({
      ...x,
      detail: (x.detectedDivergences ?? []).map((d: any) => `${d.indicatorName} (${d.type}، ${d.barsAgo} کندل قبل)`).join(' | ')
    }));
  }
  readonly cols = [
    { key: 'symbol', label: 'نماد' }, { key: 'divergenceCount', label: 'تعداد واگرایی' },
    { key: 'marketPosition', label: 'وضعیت' }, { key: 'detail', label: 'جزئیات' }
  ];
  status(): string { return this.data()?.status ?? ''; }
  isEmpty(): boolean { return !this.data()?.topPicks?.length; }
}
