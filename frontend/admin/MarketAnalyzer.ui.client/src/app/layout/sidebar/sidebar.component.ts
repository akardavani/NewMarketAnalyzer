import { NgTemplateOutlet } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, ElementRef, HostListener, inject, input, output, signal, untracked, viewChild } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter, map, startWith } from 'rxjs';

import { ITreeNode } from '@components/tree-view/tree-view.component';
import { pickMenuIcon } from './menu-icons';
import { PublishModalComponent } from '../../../modules/tools/publish-modal/publish-modal.component';
import { NotesPanelComponent } from '../../../modules/tools/notes/notes-panel.component';
import { ResourcesModalComponent } from '../../../modules/tools/resources/resources-modal.component';
import { BookmarksModalComponent } from '../../../modules/tools/bookmarks/bookmarks-modal.component';
import { PasswordsModalComponent } from '../../../modules/tools/passwords/passwords-modal.component';
import { AuthService } from '@services/auth.service';
import { DialogService } from '@services/dialog.service';
import { ChangePasswordComponent } from '../../pages/change-password/change-password.component';

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
  imports: [RouterLink, NgTemplateOutlet, PublishModalComponent, NotesPanelComponent, ResourcesModalComponent, BookmarksModalComponent, PasswordsModalComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent {
  private readonly router = inject(Router);
  readonly nodes = input.required<ReadonlyArray<ITreeNode>>();
  readonly alertCount = input(0);
  readonly userFullname = input('');
  readonly quickAction = input<{ path: string; label: string }>({ path: '', label: '' });
  readonly notificationRequested = output<void>();
  readonly searchRequested = output<void>();

  protected readonly tree = computed(() => build(this.nodes(), '', 0)
    .filter(n => !isFinancialReports(n.label))
    .sort((a, b) => Number(isSecurity(a.label)) - Number(isSecurity(b.label))));
  protected readonly mainTree = computed(() => this.tree().filter(n => !isSecurity(n.label)));
  protected readonly securityTree = computed(() => this.tree().filter(n => isSecurity(n.label)));
  protected readonly accountOpen = signal(false);
  private readonly sidebarRoot = viewChild<ElementRef<HTMLElement>>('sidebarRoot');
  private readonly auth = inject(AuthService);
  private readonly dialog = inject(DialogService);

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
  protected readonly toolsOpen = signal(false);
  protected readonly selectedSection = signal<string | null>(null);
  protected readonly submenuTop = signal(72);
  protected readonly selectedSectionNode = computed(() => this.tree().find(n => n.key === this.selectedSection()) ?? null);

  protected changePassword(): void {
    this.closeFlyouts();
    this.dialog.open(ChangePasswordComponent);
  }

  protected signOut(): void { this.auth.signOut(); }

  protected toggleAccount(): void {
    this.selectedSection.set(null);
    this.accountOpen.update(v => !v);
  }

  protected closeFlyouts(): void {
    this.accountOpen.set(false);
    this.selectedSection.set(null);
  }

  protected openNotifications(): void {
    this.closeFlyouts();
    this.notificationRequested.emit();
  }

  @HostListener('document:click', ['$event'])
  closeAccountOnOutsideClick(event: MouseEvent): void {
    const target = event.target;
    if (!(target instanceof Element) || !target.closest('.account-host, .account-flyout')) this.accountOpen.set(false);
    if (!(target instanceof Element) || !target.closest('.sb')) this.selectedSection.set(null);
  }

  protected openSection(node: NavNode, event: MouseEvent): void {
    if (this.selectedSection() === node.key) {
      this.selectedSection.set(null);
      return;
    }
    this.setFlyoutTop(event.currentTarget as HTMLElement, Math.min(500, node.children.length * 42 + 68));
    this.selectedSection.set(node.key);
  }

  protected openTools(event: MouseEvent): void {
    if (this.selectedSection() === 'tools') {
      this.selectedSection.set(null);
      return;
    }
    this.setFlyoutTop(event.currentTarget as HTMLElement, 330);
    this.selectedSection.set('tools');
  }

  private setFlyoutTop(anchor: HTMLElement, estimatedHeight: number): void {
    const sidebar = this.sidebarRoot()?.nativeElement;
    if (!sidebar) return;
    const sidebarRect = sidebar.getBoundingClientRect();
    const anchorRect = anchor.getBoundingClientRect();
    const maxTop = Math.max(12, sidebarRect.height - Math.min(estimatedHeight, sidebarRect.height - 24) - 12);
    this.submenuTop.set(Math.min(maxTop, Math.max(12, anchorRect.top - sidebarRect.top - 8)));
  }

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
    if (node.depth === 0) {
      if (node.children.length) this.selectedSection.set(this.selectedSection() === node.key ? null : node.key);
      else if (node.url) void this.router.navigateByUrl(node.url);
      return;
    }
    this.open.update(current => {
      const next = new Set(current);
      if (current.has(node.key)) {
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

function isSecurity(label: string): boolean {
  return label.trim() === 'امنیت و دسترسی';
}

function isFinancialReports(label: string): boolean {
  return label.trim() === 'گزارش‌های صورت مالی';
}
