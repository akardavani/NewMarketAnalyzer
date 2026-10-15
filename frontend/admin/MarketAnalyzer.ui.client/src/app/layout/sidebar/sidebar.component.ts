import { NgTemplateOutlet } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal, untracked } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter, map, startWith } from 'rxjs';

import { ITreeNode } from '@components/tree-view/tree-view.component';
import { BrandService } from '@services/brand.service';
import { ShellStateService } from '@services/shell-state.service';
import { pickMenuIcon } from './menu-icons';

/** گرهٔ آمادهٔ نمایش؛ key = مسیر اندیس‌ها («0»، «0.2»، ...) و depth = عمق. */
interface NavNode {
  readonly key: string;
  readonly label: string;
  readonly icon?: string;
  readonly path?: string;
  readonly url?: string;
  readonly depth: number;
  readonly children: readonly NavNode[];
}

/**
 * سایدبار: عنوان‌های منوی اصلی = بخش‌های آکاردئونی، زیرمنوها داخل آن‌ها.
 * جمع‌شده = فقط آیکون (با tooltip). باز/بسته‌بودن در ShellStateService ذخیره می‌شود (مثل Flux).
 * دادهٔ منو همان ITreeNode سرور است؛ هیچ نام یا رنگ ثابتی در این کامپوننت نیست.
 */
@Component({
  selector: 'fd-sidebar',
  imports: [RouterLink, NgTemplateOutlet],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent {
  private readonly router = inject(Router);
  protected readonly shell = inject(ShellStateService);
  protected readonly brand = inject(BrandService);

  readonly nodes = input.required<ReadonlyArray<ITreeNode>>();

  protected readonly collapsed = this.shell.collapsed;
  protected readonly tree = computed(() => build(this.nodes(), '', 0));
  protected readonly initial = this.brand.productName.trim().charAt(0);

  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map(e => e.urlAfterRedirects),
      startWith(this.router.url)
    ),
    { initialValue: this.router.url }
  );
  private readonly activePath = computed(() => trimSlashes(this.currentUrl().split(/[?#]/)[0]));

  /** کلید بخش‌هایی که آیتم فعال داخلشان است. */
  private readonly activeKeys = computed(() => findActiveKeys(this.tree(), this.activePath()));

  private readonly open = signal<ReadonlySet<string>>(new Set());

  constructor() {
    // با هر ناوبری، بخش حاوی صفحهٔ فعال باز می‌شود (در سطح اول فقط یک بخش باز می‌ماند).
    effect(() => {
      const keys = this.activeKeys();
      if (!keys.length) {
        return;
      }
      untracked(() => this.open.update(current => {
        const next = new Set([...current].filter(k => k.includes('.')));
        keys.forEach(k => next.add(k));
        return next;
      }));
    });
  }

  /** فقط آیتم‌های سطح اول آیکن دارند. */
  protected iconFor(node: NavNode): string {
    return pickMenuIcon(node.label, node.icon);
  }

  protected isOpen(node: NavNode): boolean {
    return this.open().has(node.key);
  }

  protected isActive(node: NavNode): boolean {
    return !!node.path && trimSlashes(node.path) === this.activePath();
  }

  protected hasActive(node: NavNode): boolean {
    return this.activeKeys().includes(node.key);
  }

  protected toggle(node: NavNode): void {
    // در حالت ریل، کلیک روی آیتم گروه‌دار سایدبار را باز می‌کند و همان گروه را نشان می‌دهد.
    const wasCollapsed = this.collapsed();
    if (wasCollapsed) {
      this.shell.setCollapsed(false);
    }
    this.open.update(current => {
      const next = new Set(current);
      if (current.has(node.key) && !wasCollapsed) {
        next.delete(node.key);
      } else {
        if (node.depth === 0) {
          [...next].filter(k => !k.includes('.')).forEach(k => next.delete(k));
        }
        next.add(node.key);
      }
      return next;
    });
  }
}

function build(nodes: ReadonlyArray<ITreeNode> | undefined, parentKey: string, depth: number): NavNode[] {
  return (nodes ?? []).map((n, i) => {
    const key = parentKey ? `${parentKey}.${i}` : `${i}`;
    return {
      key,
      label: n.label,
      icon: n.icon,
      path: n.path,
      url: n.path ? '/' + trimSlashes(n.path) : undefined,
      depth,
      children: build(n.children, key, depth + 1)
    };
  });
}

function findActiveKeys(nodes: readonly NavNode[], activePath: string): string[] {
  for (const n of nodes) {
    if (!n.children.length) {
      if (n.path && trimSlashes(n.path) === activePath) {
        return [];
      }
      continue;
    }
    const inner = findActiveKeys(n.children, activePath);
    if (inner.length || hasLeaf(n.children, activePath)) {
      return [n.key, ...inner];
    }
  }
  return [];
}

function hasLeaf(nodes: readonly NavNode[], activePath: string): boolean {
  return nodes.some(n => (!n.children.length && !!n.path && trimSlashes(n.path) === activePath));
}

function trimSlashes(value: string): string {
  return value.replace(/^\/+|\/+$/g, '');
}
