import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GlobalImports } from '@imports';
import { AppDataService } from '@services/app-data.service';
import { BrandService } from '@services/brand.service';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

interface DashboardSymbol {
  marketKey: string;
  symbolKey: string;
  name: string;
  currencyCode: string;
  lastPrice: number | null;
  priceTimestampUtc: string | null;
}

@Component({
  selector: 'fd-dashboard',
  imports: [...GlobalImports],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  protected readonly brand = inject(BrandService);
  protected readonly appData = inject(AppDataService);
  protected readonly symbols = signal<DashboardSymbol[]>([]);
  private readonly http = inject(HttpClient);

  ngOnInit(): void {
    this.http.get<ApiResult<DashboardSymbol[]>>(apiUrl('Dashboard/GetSymbols'))
      .subscribe(result => {
        if (result.successed) this.symbols.set(result.data ?? []);
      });
  }
}
