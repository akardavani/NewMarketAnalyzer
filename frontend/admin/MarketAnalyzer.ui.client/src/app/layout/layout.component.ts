import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { AppDataService } from '../../infrastructure/services/app-data.service';
import { BrandService } from '@services/brand.service';
import { DialogService } from '@services/dialog.service';
import { ShellStateService } from '@services/shell-state.service';
import { ITreeNode } from '@components/tree-view/tree-view.component';
import { NotificationComponent } from '../components/notification/notification.component';
import { CommandPaletteComponent } from './command-palette/command-palette.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { GlobalImports } from '@imports';
import { bind } from '@utils';

@Component({
  selector: 'fd-layout',
  imports: [...GlobalImports, RouterOutlet, SidebarComponent, CommandPaletteComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  protected readonly shell = inject(ShellStateService);
  protected readonly brand = inject(BrandService);
  private readonly dialog = inject(DialogService);
  private readonly appData = inject(AppDataService);

  protected readonly menuItems = signal<ReadonlyArray<ITreeNode>>(this.appData.menuItems);
  protected readonly userFullname = signal(this.appData.fullName ?? '');

  constructor() {
    this.appData.valueChanges.subscribe(() => {
      this.menuItems.set(this.appData.menuItems);
      this.userFullname.set(this.appData.fullName ?? '');
    });
  }

  get userAlertCount(): number {
    return this.appData.userAlerts?.length ?? 0;
  }

  openSearch(): void {
    this.shell.searchOpen.set(true);
  }

  @bind()
  openNotificationDialog(): void {
    this.dialog.open(NotificationComponent);
  }
}
