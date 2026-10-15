import { ChangeDetectorRef, Component, computed, ElementRef, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { distinctUntilChanged, filter, interval } from 'rxjs';

import { AppDataService } from '../../infrastructure/services/app-data.service';
import { AuthService } from '@services/auth.service';
import { BrandService } from '@services/brand.service';
import { DialogService } from '@services/dialog.service';
import { ShellStateService } from '@services/shell-state.service';
import { ThemeService } from '@services/theme.service';
import { ITreeNode } from '@components/tree-view/tree-view.component';
import { ThemeCustomizerComponent } from '@components/theme-customizer/theme-customizer.component';
import { ChangePasswordComponent } from '../pages/change-password/change-password.component';
import { NotificationComponent } from '../components/notification/notification.component';
import { CommandPaletteComponent } from './command-palette/command-palette.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { GlobalImports } from '@imports';
import { DateTime } from '@xobin/globalization';
import { bind, formatDateTime } from '@utils';

/**
 * پوستهٔ برنامه: سایدبار + هدر + محتوا + فوتر. هیچ نام/رنگ ثابتی ندارد؛
 * نام محصول از BrandService و رنگ‌ها از توکن‌های src/styles/theme می‌آید.
 */
@Component({
  selector: 'fd-layout',
  imports: [...GlobalImports, RouterOutlet, SidebarComponent, CommandPaletteComponent, ThemeCustomizerComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent implements OnInit {
  private readonly theme = inject(ThemeService);
  protected readonly shell = inject(ShellStateService);
  protected readonly brand = inject(BrandService);

  time!: string;
  showUserMenu = false;
  userFullname: string;
  breadcrumbs: BreadCrumb[];
  menuItems: ReadonlyArray<ITreeNode>;

  protected readonly isDark = computed(() => this.theme.resolvedMode() === 'dark');
  protected readonly searchHint = /Mac|iPhone|iPad/.test(navigator.platform) ? '⌘ K' : 'Ctrl K';

  @ViewChild('userProfile')
  userProfile!: ElementRef<HTMLElement>;

  constructor(
    private readonly _router: Router,
    private readonly dialog: DialogService,
    private readonly cdr: ChangeDetectorRef,
    private readonly authService: AuthService,
    private readonly appData: AppDataService,
    private readonly _activatedRoute: ActivatedRoute) {

    this.menuItems = appData.menuItems;
    this.userFullname = appData.fullName;

    appData.valueChanges.subscribe(() => {
      this.menuItems = appData.menuItems;
      this.userFullname = appData.fullName;
    });

    this.breadcrumbs = this.buildBreadCrumb(_activatedRoute.root);

    this.time = formatDateTime(DateTime.now, 'HH:mm:ss');
    interval(1000)
      .subscribe(() => {
        this.time = formatDateTime(DateTime.now, 'HH:mm:ss');
        cdr.markForCheck();
      });
  }

  get userAlertCount(): number {
    return this.appData.userAlerts?.length ?? 0;
  }

  get userInitials(): string {
    const parts = (this.userFullname ?? '').trim().split(/\s+/).filter(Boolean);
    return parts.length > 1 ? `${parts[0][0]}\u200c${parts[parts.length - 1][0]}` : (parts[0]?.slice(0, 2) ?? '');
  }

  ngOnInit(): void {
    this._router
      .events
      .pipe(
        filter(e => e instanceof NavigationEnd),
        distinctUntilChanged())
      .subscribe(() => {
        this.breadcrumbs = this.buildBreadCrumb(this._activatedRoute.root);
      });
  }

  @HostListener('document:click', ['$event'])
  documentClick(event: MouseEvent) {
    if (this.userProfile && !this.userProfile.nativeElement.contains(event.target as Node)) {
      this.showUserMenu = false;
    }
  }

  toggleTheme(): void {
    this.theme.update({ mode: this.isDark() ? 'light' : 'dark' });
  }

  openSearch(): void {
    this.shell.searchOpen.set(true);
  }

  openCustomizer(): void {
    this.shell.customizerOpen.set(true);
  }

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
    this.cdr.detectChanges();
  }

  changePassword(): void {
    this.showUserMenu = false;
    this.dialog.open(ChangePasswordComponent);
  }

  signout(): void {
    this.authService.signOut();
  }

  @bind()
  public openNotificationDialog(): void {
    this.dialog.open(NotificationComponent);
  }

  private buildBreadCrumb(route: ActivatedRoute, url: string = '', breadcrumbs: BreadCrumb[] = [{ title: 'خانه', url: '/', current: false }]): BreadCrumb[] {
    //If no routeConfig is avalailable we are on the root path
    let title = (route.routeConfig && (route.routeConfig.data && route.routeConfig.data['breadcrumb'])) || '';
    let path = route?.routeConfig?.path ?? '';

    // If the route is dynamic route such as ':id', remove it
    const lastRoutePart = path.split('/').pop();
    const isDynamicRoute = lastRoutePart!.startsWith(':');
    if (isDynamicRoute && !!route.snapshot) {
      const paramName = lastRoutePart!.split(':')[1];
      path = path.replace(lastRoutePart!, route.snapshot.params[paramName]);
      title = route.snapshot.params[paramName];
    }

    //In the routeConfig the complete path is not available,
    //so we rebuild it each time
    const nextUrl = path ? `${url}/${path}` : url;

    const breadcrumb: BreadCrumb = {
      title: title,
      url: nextUrl,
      current: this._activatedRoute == route.root
    };
    // Only adding route with non-empty title
    const newBreadcrumbs = breadcrumb.title ? [...breadcrumbs, breadcrumb] : [...breadcrumbs];
    if (route.firstChild) {
      //If we are not on our current path yet,
      //there will be more children to look after, to build our breadcumb
      return this.buildBreadCrumb(route.firstChild, nextUrl, newBreadcrumbs);
    }
    return newBreadcrumbs;
  }
}

interface BreadCrumb {
  title: string;
  url: string;
  current: boolean;
}
