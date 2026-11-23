import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GlobalImports } from '@imports';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';
import { map } from 'rxjs';
import { SymbolItem } from '../../../modules/MarketIntelligence/shared/strategy-api.service';
import { SymbolPickerComponent } from '../../../modules/MarketIntelligence/shared/symbol-picker/symbol-picker.component';

interface DashboardSymbolSelection {
  marketKey: string;
  symbolKey: string;
  name: string;
  currencyCode: string;
}

@Component({
  selector: 'fd-dashboard-settings',
  imports: [...GlobalImports, SymbolPickerComponent],
  templateUrl: './dashboard-settings.component.html',
  styleUrl: './dashboard-settings.component.scss'
})
export class DashboardSettingsComponent implements OnInit {
  private readonly http = inject(HttpClient);
  protected readonly selected = signal<DashboardSymbolSelection[]>([]);
  protected readonly pickerSelection = computed(() => this.selected().map(item => ({
    symbolKey: item.symbolKey,
    symbol: item.name,
    name: item.name,
    market: item.marketKey
  })));
  protected readonly message = signal('');
  protected readonly busy = signal(false);

  protected readonly searchSymbols = (text: string) => this.http
    .get<ApiResult<DashboardSymbolSelection[]>>(apiUrl('Dashboard/SearchSymbols'), { params: { text, take: 50 } })
    .pipe(map(result => result.successed ? (result.data ?? []).map(item => ({
      symbolKey: item.symbolKey,
      symbol: item.name,
      name: item.name,
      market: item.marketKey
    })) : []));

  ngOnInit(): void {
    this.http.get<ApiResult<DashboardSymbolSelection[]>>(apiUrl('Dashboard/GetSymbols'))
      .subscribe(result => {
        if (result.successed) this.selected.set((result.data ?? []).map(({ marketKey, symbolKey, name, currencyCode }) => ({ marketKey, symbolKey, name, currencyCode })));
      });
  }

  protected selectionChanged(items: SymbolItem[]): void {
    const previous = this.selected();
    this.selected.set(items.map(item => {
      const existing = previous.find(x => x.marketKey === item.market && x.symbolKey === item.symbolKey);
      return existing ?? {
        marketKey: item.market,
        symbolKey: item.symbolKey,
        name: item.name || item.symbol,
        currencyCode: ''
      };
    }));
  }

  protected save(): void {
    this.busy.set(true);
    this.message.set('');
    this.http.put<ApiResult<boolean>>(apiUrl('Dashboard/SaveSymbols'), {
      symbols: this.selected().map(({ marketKey, symbolKey }) => ({ marketKey, symbolKey }))
    }).subscribe({
      next: result => {
        this.busy.set(false);
        this.message.set(result.successed ? 'تنظیمات داشبورد ذخیره شد.' : result.errors?.join(' ') ?? 'ذخیره تنظیمات انجام نشد.');
      },
      error: () => {
        this.busy.set(false);
        this.message.set('ارتباط با سرور برقرار نشد.');
      }
    });
  }
}
