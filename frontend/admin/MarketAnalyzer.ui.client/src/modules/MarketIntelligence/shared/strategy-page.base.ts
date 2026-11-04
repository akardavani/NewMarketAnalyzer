import { Directive, inject, signal } from '@angular/core';
import { StrategyApiService } from './strategy-api.service';
import { jalaliTextToIso } from './jalali';
import { Observable } from 'rxjs';

/** Common run/loading/error handling. Each strategy page extends this and only supplies name + params. */
@Directive()
export abstract class StrategyPageBase<TData = any> {
  protected readonly api = inject(StrategyApiService);

  abstract readonly strategyName: string;
  readonly running = signal(false);
  readonly errors = signal<string[]>([]);
  readonly data = signal<TData | null>(null);

  /** Return parameters, or push messages into `problems` and return null. */
  protected abstract buildParams(problems: string[]): Record<string, unknown> | null;

  protected isoDate(label: string, text: string, problems: string[]): string | null {
    const iso = jalaliTextToIso(text ?? '');
    if (!iso) problems.push(`تاریخ "${label}" نامعتبر است (نمونه: 1405/07/26).`);
    return iso;
  }

  protected required(label: string, value: unknown, problems: string[]): void {
    if (String(value ?? '').trim() === '') problems.push(`فیلد "${label}" الزامی است.`);
  }

  protected resetState(): void {
    this.errors.set([]);
    this.data.set(null);
  }

  protected execute(request: Observable<TData>): void {
    this.resetState();
    this.running.set(true);
    request.subscribe({
      next: d => { this.running.set(false); this.data.set(d); },
      error: e => { this.running.set(false); this.errors.set([e?.error?.errors?.join(' ') || e?.message || 'خطای نامشخص']); }
    });
  }

  run(): void {
    this.resetState();
    const problems: string[] = [];
    const parameters = this.buildParams(problems);
    if (problems.length || !parameters) { this.errors.set(problems); return; }
    this.execute(this.api.run(this.strategyName, parameters));
  }
}
