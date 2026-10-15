import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthService } from '@services/auth.service';
import { ITreeNode } from '@components/tree-view/tree-view.component';
import {
  ApiResult,
  AppState,
} from '@typings';
import { apiUrl} from '@utils';
import { firstValueFrom, map, Subject, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AppDataService {
  fullName!: string;  
  appState!: AppState;
  lastLoginDate!: Date;
  menuItems: ReadonlyArray<ITreeNode> = [];
  userAlerts!: UserAlertItem[];
  valueChanges = new Subject<void>();

  constructor(
    private readonly auth: AuthService,
    private readonly client: HttpClient,
  ) {}

  init(): Promise<void> {
    if (!this.auth.authenticated) {
      return Promise.resolve();
    }

    const request = this.client
      .get<ApiResult<AppData>>(apiUrl('Home/GetAppData'))
      .pipe(
        tap((response) => {
          if (response.successed) {
            this.menuItems = response.data.menus;
            this.appState = response.data.appState;
            this.fullName = response.data.fullName;
            this.lastLoginDate = response.data.lastLoginDate;
          }
        }),
        map((_) => void 0),
      );

    return firstValueFrom(request);
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

