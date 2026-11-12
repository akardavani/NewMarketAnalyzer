import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GlobalImports } from '@imports';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

interface DashboardSymbolSelection {
  marketKey: string;
  symbolKey: string;
  name: string;
  currencyCode: string;
}

@Component({
  selector: 'fd-dashboard-settings',
  imports: [...GlobalImports],
  templateUrl: './dashboard-settings.component.html',
  styleUrl: './dashboard-settings.component.scss'
})
export class DashboardSettingsComponent implements OnInit {
  private readonly http = inject(HttpClient);
  protected readonly searchText = signal('');
  protected readonly results = signal<DashboardSymbolSelection[]>([]);
  protected readonly selected = signal<DashboardSymbolSelection[]>([]);
  protected readonly message = signal('');
  protected readonly busy = signal(false);

  ngOnInit(): void {
    this.http.get<ApiResult<DashboardSymbolSelection[]>>(apiUrl('Dashboard/GetSymbols'))
      .subscribe(result => {
        if (result.successed) this.selected.set((result.data ?? []).map(({ marketKey, symbolKey, name, currencyCode }) => ({ marketKey, symbolKey, name, currencyCode })));
      });
  }

  protected search(): void {
    const text = this.searchText().trim();
    if (!text) {
      this.results.set([]);
      return;
    }
    this.http.get<ApiResult<DashboardSymbolSelection[]>>(apiUrl('Dashboard/SearchSymbols'), { params: { text, take: 50 } })
      .subscribe(result => this.results.set(result.successed ? result.data ?? [] : []));
  }

  protected add(item: DashboardSymbolSelection): void {
    if (this.selected().some(x => x.marketKey === item.marketKey && x.symbolKey === item.symbolKey)) return;
    this.selected.update(items => [...items, item]);
  }

  protected remove(item: DashboardSymbolSelection): void {
    this.selected.update(items => items.filter(x => x.marketKey !== item.marketKey || x.symbolKey !== item.symbolKey));
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
