import { Component, Input } from '@angular/core';

export interface ChartPoint { label: string; value: number; }
export interface ChartSeriesInput { label: string; points: ChartPoint[]; }

const COLORS = ['#0d6efd', '#dc3545', '#198754', '#fd7e14', '#6f42c1', '#20c997', '#d63384', '#6c757d'];

@Component({
  selector: 'mi-line-chart',
  templateUrl: './line-chart.component.html',
  styleUrl: './line-chart.component.scss'
})
export class LineChartComponent {
  readonly W = 1000; readonly H = 420;
  readonly padL = 70; readonly padR = 70; readonly padT = 30; readonly padB = 50;
  @Input() title = 'نمودار';
  @Input() set series(v: ChartSeriesInput[] | null) { this._series = v ?? []; this.build(); }
  private _series: ChartSeriesInput[] = [];

  model: { items: { label: string; color: string; points: string }[]; xLabels: string[]; yMin: number; yMax: number } | null = null;

  private build(): void {
    const s = this._series.filter(x => x.points.length);
    if (!s.length) { this.model = null; return; }
    const all = s.flatMap(x => x.points.map(p => p.value));
    const yMin = Math.min(...all), yMax = Math.max(...all);
    const span = yMax - yMin || 1;
    const longest = s.reduce((a, b) => (b.points.length > a.points.length ? b : a));
    const n = Math.max(longest.points.length - 1, 1);
    const { W, H, padL, padR, padT, padB } = this;
    this.model = {
      yMin, yMax,
      xLabels: [0, Math.floor(n / 2), n].map(i => longest.points[i]?.label ?? ''),
      items: s.map((x, i) => ({
        label: x.label,
        color: COLORS[i % COLORS.length],
        points: x.points.map((p, j) =>
          `${(padL + (j / n) * (W - padL - padR)).toFixed(1)},${(H - padB - ((p.value - yMin) / span) * (H - padT - padB)).toFixed(1)}`).join(' ')
      }))
    };
  }
}
