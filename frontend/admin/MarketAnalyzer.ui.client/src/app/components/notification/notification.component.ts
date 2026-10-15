import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { AppDataService, UserAlertItem } from '@services/app-data.service';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss']
})
export class NotificationComponent {

  userAlert: UserAlertItem[];

  constructor(
    appData: AppDataService,
    public dialogRef: MatDialogRef<NotificationComponent>
  ) {
    this.userAlert = appData.userAlerts ?? [];
  }

  public close(): void {
    this.dialogRef.close();
  }
}
