import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StrategyPageBase } from '../../shared/strategy-page.base';
import { daysAgo, today } from '../../shared/jalali';
import { DataTableComponent } from '../../shared/data-table/data-table.component';
import { RunIranQueueRequest } from '../../shared/strategy-api.service';

@Component({
  selector: 'mi-iran-queue',
  imports: [FormsModule, DataTableComponent],
  templateUrl: './iran-queue.component.html',
  styleUrl: './iran-queue.component.scss'
})
export class IranQueueComponent extends StrategyPageBase {
  readonly strategyName = 'IranQueue';
  inputFilePath = '';
  fromRatio = 5;
  toRatio = 15;
  return1Month = 50;

  protected buildParams(p: string[]) {
    this.required('مسیر فایل اکسل', this.inputFilePath, p);
    return p.length ? null : {
      InputFilePath: this.inputFilePath.trim(),
      FromRatio: Number(this.fromRatio), ToRatio: Number(this.toRatio), Return1Month: Number(this.return1Month)
    };
  }

  override run(): void {
    this.resetState();
    const problems: string[] = [];
    const params = this.buildParams(problems);
    if (problems.length || !params) { this.errors.set(problems); return; }

    const request: RunIranQueueRequest = {
      inputFilePath: String(params['InputFilePath']),
      fromRatio: Number(params['FromRatio']),
      toRatio: Number(params['ToRatio']),
      return1Month: Number(params['Return1Month'])
    };
    this.execute(this.api.runIranQueue(request));
  }

  readonly cols = [
    { key: 'symbol', label: 'نماد' }, { key: 'ratio', label: 'نسبت' }, { key: 'demandVolume', label: 'حجم تقاضا' },
    { key: 'volumeToday', label: 'حجم امروز' }, { key: 'avgVolume30', label: 'میانگین حجم ۳۰ روز' }, { key: 'return1Month', label: 'بازدهی یک ماهه' }
  ];
  status(): string { return this.data()?.status ?? ''; }
  isEmpty(): boolean { return !this.data()?.queueResult?.length; }
}
