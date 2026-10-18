import { JsonPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';

@Component({
  selector: 'fd-strategies',
  imports: [FormsModule, JsonPipe],
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
  parametersText = '{}';

  ngOnInit(): void {
    this.http.get<ApiResult<string[]>>(apiUrl('Strategy/GetAll')).subscribe(res => {
      if (res.successed) {
        this.strategies.set(res.data);
        this.selected = res.data[0] ?? '';
      } else {
        this.errors.set(res.errors);
      }
    });
  }

  run(): void {
    this.errors.set([]);
    this.result.set(null);

    let parameters: Record<string, unknown>;
    try {
      parameters = JSON.parse(this.parametersText || '{}');
    } catch {
      this.errors.set(['فرمت JSON پارامترها نامعتبر است.']);
      return;
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
