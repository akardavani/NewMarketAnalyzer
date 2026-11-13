import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GlobalImports } from '@imports';
import { LearnApiService, LearnOverview, DueLearningReview } from '../../learn-api.service';

@Component({
  selector: 'learn-overview-page',
  imports: [...GlobalImports, RouterLink],
  templateUrl: './learn-overview.component.html',
  styleUrl: './learn-overview.component.scss'
})
export class LearnOverviewComponent implements OnInit {
  private readonly api = inject(LearnApiService);
  protected readonly overview = signal<LearnOverview | null>(null);
  protected readonly dueReviews = signal<DueLearningReview[]>([]);
  protected readonly message = signal('');

  ngOnInit(): void {
    this.api.overview().subscribe({
      next: result => this.overview.set(result.successed ? result.data ?? null : null),
      error: () => this.message.set('دریافت نمای کلی یادگیری انجام نشد.')
    });
    this.loadDueReviews();
  }

  protected loadDueReviews(): void {
    this.api.dueReviews().subscribe({
      next: result => this.dueReviews.set(result.successed ? result.data ?? [] : []),
      error: () => this.message.set('دریافت فهرست مرورها انجام نشد.')
    });
  }

  protected review(item: DueLearningReview, rating: number): void {
    this.api.review(item.collectionId, item.learningItemId, rating).subscribe({
      next: result => {
        this.message.set(result.successed ? 'مرور ثبت شد و موعد بعدی تنظیم شد.' : result.errors?.join(' ') ?? 'ثبت مرور انجام نشد.');
        this.loadDueReviews();
        this.api.overview().subscribe(response => { if (response.successed) this.overview.set(response.data ?? null); });
      },
      error: () => this.message.set('ثبت مرور انجام نشد.')
    });
  }
}
