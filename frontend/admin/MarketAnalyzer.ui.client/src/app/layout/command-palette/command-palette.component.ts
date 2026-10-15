import { ChangeDetectionStrategy, Component, computed, effect, ElementRef, HostListener, inject, input, signal, viewChild } from '@angular/core';
import { Router } from '@angular/router';

import { ITreeNode } from '@components/tree-view/tree-view.component';
import { ShellStateService } from '@services/shell-state.service';

interface Entry {
  readonly label: string;
  readonly trail: string;
  readonly url: string;
  readonly haystack: string;
}

const MAX_RESULTS = 8;

/** جستجوی سریع در منو (Ctrl K). فقط ناوبری؛ دادهٔ منو از ورودی می‌آید. */
@Component({
  selector: 'fd-command-palette',
  imports: [],
  templateUrl: './command-palette.component.html',
  styleUrl: './command-palette.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CommandPaletteComponent {
  private readonly router = inject(Router);
  private readonly shell = inject(ShellStateService);

  readonly nodes = input.required<ReadonlyArray<ITreeNode>>();

  protected readonly isOpen = this.shell.searchOpen;
  protected readonly query = signal('');
  protected readonly cursor = signal(0);

  private readonly field = viewChild<ElementRef<HTMLInputElement>>('field');
  private readonly entries = computed(() => flatten(this.nodes(), []));

  protected readonly results = computed<readonly Entry[]>(() => {
    const q = normalize(this.query().trim());
    const all = this.entries();
    const hits = q ? all.filter(e => e.haystack.includes(q)) : all;
    return hits.slice(0, MAX_RESULTS);
  });

  constructor() {
    effect(() => {
      if (this.isOpen()) {
        this.query.set('');
        this.cursor.set(0);
        setTimeout(() => this.field()?.nativeElement.focus());
      }
    });
  }

  @HostListener('document:keydown', ['$event'])
  onKeydown(event: KeyboardEvent): void {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
      event.preventDefault();
      this.isOpen.update(v => !v);
    }
  }

  protected onInput(value: string): void {
    this.query.set(value);
    this.cursor.set(0);
  }

  protected onFieldKeydown(event: KeyboardEvent): void {
    const count = this.results().length;
    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.cursor.update(i => (count ? (i + 1) % count : 0));
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.cursor.update(i => (count ? (i - 1 + count) % count : 0));
        break;
      case 'Enter': {
        event.preventDefault();
        const target = this.results()[this.cursor()];
        if (target) {
          this.go(target);
        }
        break;
      }
      case 'Escape':
        event.preventDefault();
        this.close();
        break;
    }
  }

  protected go(entry: Entry): void {
    this.close();
    void this.router.navigateByUrl(entry.url);
  }

  protected close(): void {
    this.isOpen.set(false);
  }
}

function flatten(nodes: ReadonlyArray<ITreeNode> | undefined, trail: string[]): Entry[] {
  const out: Entry[] = [];
  for (const n of nodes ?? []) {
    if (n.children?.length) {
      out.push(...flatten(n.children, [...trail, n.label]));
    } else if (n.path) {
      const trailText = trail.join(' › ');
      out.push({
        label: n.label,
        trail: trailText,
        url: '/' + n.path.replace(/^\/+/, ''),
        haystack: normalize(`${n.label} ${trailText}`)
      });
    }
  }
  return out;
}

/** یکسان‌سازی «ي/ك» عربی و حروف کوچک/بزرگ برای جستجو. */
function normalize(value: string): string {
  return value.replace(/ي/g, 'ی').replace(/ك/g, 'ک').toLowerCase();
}
