import { HttpClient } from '@angular/common/http';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

interface SelectOption {
  text: string;
  value: string;
}

interface StrategyField {
  name: string;
  label: string;
  type: 'text' | 'number' | 'date' | 'select' | 'symbols';
  defaultValue?: string | number;
  options?: SelectOption[];
  required?: boolean;
  hint?: string;
}

interface StrategySchema {
  title: string;
  fields: StrategyField[];
}

// Jalali <-> Gregorian (jalaali-js algorithm)
const div = (a: number, b: number): number => Math.trunc(a / b);
const g2j = (gy: number, gm: number, gd: number): [number, number, number] => {
  const gdm = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334];
  const gy2 = gm > 2 ? gy + 1 : gy;
  let days = 355666 + 365 * gy + div(gy2 + 3, 4) - div(gy2 + 99, 100) + div(gy2 + 399, 400) + gd + gdm[gm - 1];
  let jy = -1595 + 33 * div(days, 12053);
  days %= 12053;
  jy += 4 * div(days, 1461);
  days %= 1461;
  if (days > 365) { jy += div(days - 1, 365); days = (days - 1) % 365; }
  const jm = days < 186 ? 1 + div(days, 31) : 7 + div(days - 186, 30);
  const jd = 1 + (days < 186 ? days % 31 : (days - 186) % 30);
  return [jy, jm, jd];
};
const j2g = (jy: number, jm: number, jd: number): [number, number, number] => {
  jy += 1595;
  let days = -355668 + 365 * jy + div(jy, 33) * 8 + div((jy % 33) + 3, 4) + jd + (jm < 7 ? (jm - 1) * 31 : (jm - 7) * 30 + 186);
  let gy = 400 * div(days, 146097);
  days %= 146097;
  if (days > 36524) {
    gy += 100 * div(--days, 36524);
    days %= 36524;
    if (days >= 365) days++;
  }
  gy += 4 * div(days, 1461);
  days %= 1461;
  if (days > 365) { gy += div(days - 1, 365); days = (days - 1) % 365; }
  let gd = days + 1;
  const feb = (gy % 4 === 0 && gy % 100 !== 0) || gy % 400 === 0 ? 29 : 28;
  const md = [0, 31, feb, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
  let gm = 0;
  while (gm < 13 && gd > md[gm]) gd -= md[gm++];
  return [gy, gm, gd];
};
const pad2 = (n: number): string => String(n).padStart(2, '0');
const toJalaliText = (d: Date): string => {
  const [y, m, dd] = g2j(d.getFullYear(), d.getMonth() + 1, d.getDate());
  return `${y}/${pad2(m)}/${pad2(dd)}`;
};
const normDigits = (s: string): string =>
  s.replace(/[۰-۹]/g, c => String(c.charCodeAt(0) - 1776)).replace(/[٠-٩]/g, c => String(c.charCodeAt(0) - 1632));
/** Jalali 'yyyy/MM/dd' -> Gregorian 'yyyy-MM-dd', or null if invalid. */
const jalaliTextToIso = (text: string): string | null => {
  const m = /^(\d{4})[\/\-.](\d{1,2})[\/\-.](\d{1,2})$/.exec(normDigits(text.trim()));
  if (!m) return null;
  const [jy, jm, jd] = [+m[1], +m[2], +m[3]];
  if (jm < 1 || jm > 12 || jd < 1 || jd > 31) return null;
  const [gy, gm, gd] = j2g(jy, jm, jd);
  return `${gy}-${pad2(gm)}-${pad2(gd)}`;
};

const daysAgo = (days: number): string => {
  const d = new Date();
  d.setDate(d.getDate() - days);
  return toJalaliText(d);
};

const today = (): string => toJalaliText(new Date());

const TIMEFRAME_OPTIONS: SelectOption[] = [
  { text: 'Daily', value: 'Daily' }
];

const EXCEL_FIELD: StrategyField = {
  name: 'InputFilePath', label: 'مسیر فایل اکسل', type: 'text', required: true,
  hint: 'مسیر کامل فایل روی سرور'
};

// Mirrors legacy StrategyUi/StrategyInputs/*Input.cs. Keys must match strategy names returned by Strategy/GetAll.
interface SymbolItem { symbolKey: string; symbol: string; name: string; market: string; }
interface ChartSeries { label: string; color: string; points: string; }
interface ChartModel { series: ChartSeries[]; xLabels: string[]; yMin: number; yMax: number; }
const COLORS = ['#0d6efd', '#dc3545', '#198754', '#fd7e14', '#6f42c1', '#20c997', '#d63384', '#6c757d'];
const W = 1000, H = 420, PADL = 70, PADR = 70, PADT = 30, PADB = 50;

const SCHEMAS: Record<string, StrategySchema> = {
  IranEquityMain: {
    title: 'پیدا کردن سهم با اکسل ایزی',
    fields: [
      EXCEL_FIELD,
      { name: 'StartDate', label: 'تاریخ شروع', type: 'date', defaultValue: daysAgo(30), required: true },
      { name: 'EndDate', label: 'تاریخ پایان', type: 'date', defaultValue: today(), required: true }
    ]
  },
  BottomReversal: {
    title: 'برگشت از کف',
    fields: [
      EXCEL_FIELD,
      { name: 'StartDate', label: 'تاریخ شروع', type: 'date', defaultValue: daysAgo(30), required: true },
      { name: 'EndDate', label: 'تاریخ پایان', type: 'date', defaultValue: today(), required: true }
    ]
  },
  IranQueue: {
    title: 'صف خرید',
    fields: [
      EXCEL_FIELD,
      { name: 'FromRatio', label: 'از نسبت', type: 'number', defaultValue: 5, required: true },
      { name: 'ToRatio', label: 'تا نسبت', type: 'number', defaultValue: 15, required: true },
      { name: 'Return1Month', label: 'بازدهی یک ماهه', type: 'number', defaultValue: 50, required: true }
    ]
  },
  MultiIndicatorDivergence: {
    title: 'واگرایی اندیکاتور ها',
    fields: [EXCEL_FIELD]
  },
  RatioCandles: {
    title: 'نسبت نمادها',
    fields: [
      { name: 'Symbols', label: 'نمادها', type: 'symbols', required: true },
      { name: 'StartDate', label: 'تاریخ شروع', type: 'date', defaultValue: daysAgo(30), required: true },
      { name: 'EndDate', label: 'تاریخ پایان', type: 'date', defaultValue: today(), required: true },
      { name: 'Timeframe', label: 'تایم‌فریم', type: 'select', defaultValue: 'Daily', options: TIMEFRAME_OPTIONS, required: true }
    ]
  },
  YtmFuture: {
    title: 'YTM پیدا کردن',
    fields: [
      { name: 'StartDate', label: 'تاریخ شروع', type: 'date', defaultValue: daysAgo(30), required: true },
      { name: 'EndDate', label: 'تاریخ پایان', type: 'date', defaultValue: today(), required: true },
      {
        name: 'Symbol', label: 'نماد', type: 'select', defaultValue: 'ETC', required: true,
        options: [
          { text: 'طلای لوتوس', value: 'ETC' },
          { text: 'سپرده نقره', value: 'SIL' }
        ]
      },
      {
        name: 'Calculates', label: 'محاسبات', type: 'select', defaultValue: 'NearestYtm', required: true,
        options: [
          { text: 'سر رسید نزدیک YTM', value: 'NearestYtm' },
          { text: 'سر رسید دوم YTM', value: 'SecondYtm' },
          { text: 'سر رسید دور YTM', value: 'LatestYtm' },
          { text: 'سر رسید نزدیک', value: 'NearestContract' },
          { text: 'سر رسید دوم', value: 'SecondContract' },
          { text: 'سر رسید دور', value: 'LatestContract' },
          { text: 'سود', value: 'Sood' }
        ]
      }
    ]
  }
};

@Component({
  selector: 'fd-strategies',
  imports: [FormsModule],
  templateUrl: './strategies.component.html',
  styleUrl: './strategies.component.scss'
})
export class StrategiesComponent implements OnInit {
  private readonly http = inject(HttpClient);

  readonly strategies = signal<string[]>([]);
  readonly running = signal(false);
  readonly errors = signal<string[]>([]);
  readonly result = signal<unknown>(null);

  selected = '';
  values: Record<string, string | number> = {};

  // symbol picker
  selectedSymbols: SymbolItem[] = [];
  symbolText = '';
  readonly symbolOptions = signal<SymbolItem[]>([]);
  private searchTimer?: ReturnType<typeof setTimeout>;

  readonly payload = computed(() => {
    const r = this.result() as { data?: unknown } | null;
    return r && typeof r === 'object' && 'data' in r ? r.data : r;
  });

  readonly chart = computed<ChartModel | null>(() => {
    const d = this.payload() as any;
    if (!Array.isArray(d) || !d.length) return null;
    const raw: { label: string; rows: any[] }[] = Array.isArray(d[0]?.results)
      ? d.map((x: any) => ({ label: this.symbolLabel(x.symbol ?? x.instrumentId), rows: x.results }))
      : ('growth' in d[0] || 'value' in d[0]) && 'persianDate' in d[0]
        ? [{ label: this.selected, rows: d }]
        : [];
    if (!raw.length) return null;
    const val = (r: any) => Number(r.growth ?? r.value ?? 0);
    const all = raw.flatMap(s => s.rows.map(val));
    const yMin = Math.min(...all), yMax = Math.max(...all);
    const span = yMax - yMin || 1;
    const longest = raw.reduce((a, b) => (b.rows.length > a.rows.length ? b : a));
    const n = Math.max(longest.rows.length - 1, 1);
    const series = raw.map((s, i) => ({
      label: s.label,
      color: COLORS[i % COLORS.length],
      points: s.rows.map((r, j) =>
        `${(PADL + (j / n) * (W - PADL - PADR)).toFixed(1)},${(H - PADB - ((val(r) - yMin) / span) * (H - PADT - PADB)).toFixed(1)}`
      ).join(' ')
    }));
    const xLabels = [0, Math.floor(n / 2), n].map(i => longest.rows[i]?.persianDate ?? '');
    return { series, xLabels, yMin, yMax };
  });

  readonly table = computed(() => {
    const d = this.payload() as any;
    if (!Array.isArray(d) || !d.length || typeof d[0] !== 'object' || Array.isArray(d[0]?.results) || this.chart()) return null;
    const cols = Object.keys(d[0]).filter(k => typeof d[0][k] !== 'object' || d[0][k] === null);
    return { cols, rows: d as Record<string, unknown>[] };
  });

  SCHEMAS_TITLE(name: string): string {
    return SCHEMAS[name]?.title ?? name;
  }

  readonly chartW = W;
  readonly chartH = H;
  readonly padL = PADL;
  readonly padR = PADR;
  readonly padT = PADT;
  readonly padB = PADB;

  private symbolLabel(key: string): string {
    const s = this.selectedSymbols.find(x => x.symbolKey === key || x.symbol === key);
    return s ? `${s.name || s.symbol} (${s.symbolKey})` : key;
  }

  onSymbolInput(): void {
    clearTimeout(this.searchTimer);
    if (!this.symbolText.trim()) { this.symbolOptions.set([]); return; }
    this.searchTimer = setTimeout(() => {
      this.http.get<ApiResult<SymbolItem[]>>(apiUrl('Strategy/SearchSymbols'), { params: { text: this.symbolText.trim() } })
        .subscribe(res => this.symbolOptions.set(res.successed ? res.data : []));
    }, 300);
  }

  addSymbol(item: SymbolItem): void {
    if (!this.selectedSymbols.some(x => x.symbolKey === item.symbolKey)) this.selectedSymbols = [...this.selectedSymbols, item];
    this.symbolText = '';
    this.symbolOptions.set([]);
  }

  removeSymbol(item: SymbolItem): void {
    this.selectedSymbols = this.selectedSymbols.filter(x => x !== item);
  }

  get schema(): StrategySchema | null {
    return SCHEMAS[this.selected] ?? null;
  }

  ngOnInit(): void {
    this.http.get<ApiResult<string[]>>(apiUrl('Strategy/GetAll')).subscribe(res => {
      if (res.successed) {
        this.strategies.set(res.data);
        this.onStrategyChange(res.data[0] ?? '');
      } else {
        this.errors.set(res.errors);
      }
    });
  }

  onStrategyChange(name: string): void {
    this.selected = name;
    this.values = {};
    this.selectedSymbols = [];
    this.symbolOptions.set([]);
    this.errors.set([]);
    this.result.set(null);
    for (const field of this.schema?.fields ?? []) {
      this.values[field.name] = field.defaultValue ?? '';
    }
  }

  run(): void {
    this.errors.set([]);
    this.result.set(null);

    const missing = (this.schema?.fields ?? [])
      .filter(f => f.required && (f.type === 'symbols' ? !this.selectedSymbols.length : String(this.values[f.name] ?? '').trim() === ''))
      .map(f => `فیلد "${f.label}" الزامی است.`);
    if (missing.length) {
      this.errors.set(missing);
      return;
    }

    const badDates = (this.schema?.fields ?? [])
      .filter(f => f.type === 'date' && !jalaliTextToIso(String(this.values[f.name] ?? '')))
      .map(f => `تاریخ "${f.label}" نامعتبر است (نمونه: 1405/07/26).`);
    if (badDates.length) {
      this.errors.set(badDates);
      return;
    }

    const parameters: Record<string, unknown> = {};
    for (const field of this.schema?.fields ?? []) {
      const raw = this.values[field.name];
      switch (field.type) {
        case 'number':
          parameters[field.name] = Number(raw);
          break;
        case 'symbols':
          parameters[field.name] = this.selectedSymbols.map(x => x.symbolKey);
          break;
        case 'date':
          parameters[field.name] = jalaliTextToIso(String(raw));
          break;
        default:
          parameters[field.name] = raw;
      }
    }

    this.running.set(true);
    this.http.post<ApiResult<unknown>>(apiUrl('Strategy/Run'), { strategy: this.selected, parameters })
      .subscribe({
        next: res => {
          this.running.set(false);
          if (res.successed) this.result.set(res.data);
          else this.errors.set(res.errors);
        },
        error: err => {
          this.running.set(false);
          this.errors.set([err?.error?.errors?.join(' ') || err?.message || 'خطای نامشخص']);
        }
      });
  }
}
