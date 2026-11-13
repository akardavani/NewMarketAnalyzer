import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GlobalImports } from '@imports';
import { LearnApiService, LearningCollection } from '../../learn-api.service';

@Component({
  selector: 'learn-collections-page',
  imports: [...GlobalImports, RouterLink],
  templateUrl: './learn-collections.component.html',
  styleUrl: './learn-collections.component.scss'
})
export class LearnCollectionsComponent implements OnInit {
  private readonly api = inject(LearnApiService);
  protected readonly collections = signal<LearningCollection[]>([]);
  protected readonly title = signal('');
  protected readonly description = signal('');
  protected readonly message = signal('');
  protected readonly busy = signal(false);

  ngOnInit(): void { this.load(); }

  protected load(): void {
    this.api.collections().subscribe({
      next: result => this.collections.set(result.successed ? result.data ?? [] : []),
      error: () => this.message.set('دریافت مجموعه‌ها انجام نشد.')
    });
  }

  protected create(): void {
    const title = this.title().trim();
    if (!title) { this.message.set('عنوان مجموعه را وارد کنید.'); return; }
    this.busy.set(true);
    this.api.createCollection(title, this.description().trim()).subscribe({
      next: result => {
        this.busy.set(false);
        if (!result.successed) { this.message.set(result.errors?.join(' ') ?? 'ساخت مجموعه ناموفق بود.'); return; }
        this.title.set(''); this.description.set(''); this.message.set('مجموعه ساخته شد.'); this.load();
      },
      error: () => { this.busy.set(false); this.message.set('ساخت مجموعه انجام نشد.'); }
    });
  }

  protected archive(item: LearningCollection): void {
    this.api.archiveCollection(item.id).subscribe({
      next: result => { this.message.set(result.successed ? 'مجموعه بایگانی شد.' : result.errors?.join(' ') ?? 'بایگانی ناموفق بود.'); this.load(); },
      error: () => this.message.set('بایگانی مجموعه انجام نشد.')
    });
  }
}
