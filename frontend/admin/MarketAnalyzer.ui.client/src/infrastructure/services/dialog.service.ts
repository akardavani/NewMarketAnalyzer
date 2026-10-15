import { Injectable, Injector, Type } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { IRefreshableHost, REFRESHABLE_HOST } from '@form-utils';

@Injectable({ providedIn: 'root' })
export class DialogService {

  constructor(private readonly _dialog: MatDialog, private readonly _injector: Injector) {
  }

  open<T, R = any>(component: Type<T>): MatDialogRef<T, R>;
  open<T, R = any>(component: Type<T>, configure: Configure): MatDialogRef<T, R>;
  open<T, R = any>(component: Type<T>, host: IRefreshableHost, configure?: Configure): MatDialogRef<T, R>;
  open<T, R = any>(component: Type<T>, host?: IRefreshableHost | Configure, configure?: Configure): MatDialogRef<T, R> {

    if (typeof (host) === 'function') {
      return this.open(component, <any>null, host);
    }

    const config: MatDialogConfig = {};
    this.configDialog(config, host);
    if (typeof configure === 'function') {
      configure(config);
    }

    return this._dialog.open(component, config);
  }

  private configDialog(config: MatDialogConfig, host?: IRefreshableHost | Configure): void {
    config.panelClass = 'fd-dialog';
    config.disableClose = true;
    config.maxHeight = '98%';
    config.maxWidth = '98%';
    config.autoFocus = false;
    config.position = { top: "80px" };
    config.minWidth = "600px";
    config.minHeight = "400px";

    if (isRefreshableHost(host)) {
      config.injector = Injector.create({
        parent: this._injector,
        providers: [{ provide: REFRESHABLE_HOST, useValue: host }]
      });
    }
  }

}

type Configure = (confg: MatDialogConfig) => void;

function isRefreshableHost(obj: any): obj is IRefreshableHost {
  return obj && typeof obj === 'object' && typeof obj['refresh'] === 'function';
}
