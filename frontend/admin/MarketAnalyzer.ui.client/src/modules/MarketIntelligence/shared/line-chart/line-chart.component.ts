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
  readonly padL = 70; readonly padR = 30; readonly padT = 30; readonly padB = 90;
  tip: { cx: number; cy: number; left: number; top: number; date: string; value: string; color: string } | null = null;
  @Input() title = 'نمودار';
  @Input() set series(v: ChartSeriesInput[] | null) { this._series = v ?? []; this.build(); }
  private _series: ChartSeriesInput[] = [];

  model: { W: number; items: { label: string; color: string; points: string; pts: { x: number; y: number; label: string; value: number }[] }[];
    xTicks: { x: number; label: string }[]; yTicks: { y: number; text: string }[] } | null = null;

  hover(e: MouseEvent, s: { color: string; pts: { x: number; y: number; label: string; value: number }[] }): void {
    const svg = (e.currentTarget as SVGElement).ownerSVGElement!;
    const r = svg.getBoundingClientRect();
    const k = (this.model!.W) / r.width;
    const mx = (e.clientX - r.left) * k;
    let best = s.pts[0];
    for (const p of s.pts) if (Math.abs(p.x - mx) < Math.abs(best.x - mx)) best = p;
    const wrap = svg.parentElement!.getBoundingClientRect();
    this.tip = { cx: best.x, cy: best.y, left: e.clientX - wrap.left + 12, top: e.clientY - wrap.top - 40,
      date: best.label, value: String(+best.value.toFixed(4)), color: s.color };
  }

  private build(): void {
    const s = this._series.filter(x => x.points.length);
    if (!s.length) { this.model = null; return; }
    const all = s.flatMap(x => x.points.map(p => p.value));
    const yMin = Math.min(...all), yMax = Math.max(...all);
    const span = yMax - yMin || 1;
    const longest = s.reduce((a, b) => (b.points.length > a.points.length ? b : a));
    const n = Math.max(longest.points.length - 1, 1);
    const { H, padL, padR, padT, padB } = this;
    const W = Math.max(1000, padL + padR + n * 14);
    const X = (j: number) => padL + (j / n) * (W - padL - padR);
    const Y = (v: number) => H - padB - ((v - yMin) / span) * (H - padT - padB);
    const step = Math.max(1, Math.ceil(n / ((W - padL - padR) / 14)));
    const T = 10;
    this.model = {
      W,
      yTicks: Array.from({ length: T + 1 }, (_, i) => { const v = yMin + (span * i) / T; return { y: Y(v), text: v.toFixed(2) }; }),
      xTicks: longest.points.map((p, i) => ({ x: X(i), label: p.label })).filter((_, i) => i % step === 0),
      items: s.map((x, i) => {
        const pts = x.points.map((p, j) => ({ x: X(j), y: Y(p.value), label: p.label, value: p.value }));
        return { label: x.label, color: COLORS[i % COLORS.length], pts, points: pts.map(p => `${p.x.toFixed(1)},${p.y.toFixed(1)}`).join(' ') };
      })
    };
  }
}
