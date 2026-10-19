import { Component, Input } from '@angular/core';

export interface TableColumn { key: string; label?: string; }

/** Generic table: explicit columns, or auto columns from scalar props of the first row. Supports dotted keys. */
@Component({
  selector: 'mi-data-table',
  templateUrl: './data-table.component.html',
  styleUrl: './data-table.component.scss'
})
export class DataTableComponent {
  @Input() rows: any[] | null | undefined = [];
  @Input() columns: TableColumn[] | null = null;
  @Input() title = '';

  get cols(): TableColumn[] {
    if (this.columns?.length) return this.columns;
    const first = this.rows?.[0];
    if (!first || typeof first !== 'object') return [];
    return Object.keys(first)
      .filter(k => first[k] === null || typeof first[k] !== 'object' || (Array.isArray(first[k]) && first[k].every((x: any) => typeof x !== 'object')))
      .map(key => ({ key }));
  }

  cell(row: any, key: string): string {
    const v = key.split('.').reduce((o, k) => (o == null ? o : o[k]), row);
    if (v === null || v === undefined) return '';
    if (typeof v === 'number') return Number.isInteger(v) ? String(v) : v.toFixed(2);
    if (Array.isArray(v)) return v.map(x => (typeof x === 'number' && !Number.isInteger(x) ? x.toFixed(2) : x)).join(' ، ');
    if (typeof v === 'boolean') return v ? '✓' : '✗';
    return String(v);
  }
}
