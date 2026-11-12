import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { AppDataService } from '../../infrastructure/services/app-data.service';
import { BrandService } from '@services/brand.service';
import { DialogService } from '@services/dialog.service';
import { ShellStateService } from '@services/shell-state.service';
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

  protected readonly menuItems = this.appData.menuItems;
  protected readonly userFullname = this.appData.fullName;

  get userAlertCount(): number {
    return this.appData.userAlerts().length;
  }

  openSearch(): void {
    this.shell.searchOpen.set(true);
  }

  @bind()
  openNotificationDialog(): void {
    this.dialog.open(NotificationComponent);
  }
}
