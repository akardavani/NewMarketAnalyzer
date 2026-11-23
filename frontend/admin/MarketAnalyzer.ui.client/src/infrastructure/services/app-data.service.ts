import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { AuthService } from '@services/auth.service';
import { ITreeNode } from '@components/tree-view/tree-view.component';
import {
  ApiResult,
  AppState,
} from '@typings';
import { apiUrl} from '@utils';
import { firstValueFrom, map, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AppDataService {
  readonly fullName = signal('');
  readonly appState = signal<AppState | undefined>(undefined);
  readonly lastLoginDate = signal<Date | undefined>(undefined);
  readonly menuItems = signal<ReadonlyArray<ITreeNode>>([]);
  readonly userAlerts = signal<ReadonlyArray<UserAlertItem>>([]);
  private initialization?: Promise<void>;

  constructor(
    private readonly auth: AuthService,
    private readonly client: HttpClient,
  ) {}

  init(): Promise<void> {
    if (!this.auth.authenticated) {
      return Promise.resolve();
    }
    if (this.initialization) {
      return this.initialization;
    }

    const request = this.client
      .get<ApiResult<AppData>>(apiUrl('Home/GetAppData'))
      .pipe(
        tap((response) => {
          if (response.successed) {
            this.menuItems.set(response.data.menus ?? []);
            this.appState.set(response.data.appState);
            this.fullName.set(response.data.fullName ?? '');
            this.lastLoginDate.set(response.data.lastLoginDate);
            this.userAlerts.set(response.data.userAlerts ?? []);
          }
        }),
        map((_) => void 0),
      );

    this.initialization = firstValueFrom(request).catch(error => {
      this.initialization = undefined;
      throw error;
    });
    return this.initialization;
  }
}
interface AppData {
  fullName: string;
  lastLoginDate: Date;
  menus: ITreeNode[];
  appState: AppState;
  
  userAlerts: UserAlertItem[];
}

export interface UserAlertItem {
  id: number;
  title: string;
  message: string;
}

