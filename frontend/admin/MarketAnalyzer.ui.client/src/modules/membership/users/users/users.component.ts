import { Component, Type } from '@angular/core';
import { UsersService } from './users.service';
import { GridImports } from '@imports';
import { Users, UsersFilter } from '../../membership-models';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ColDef, ICellRendererParams } from 'ag-grid-enterprise';
import { defineActionCellParams, firstCellValueGetter, IActionCommand } from '@grid-utils';
import { CheckedCellRendererComponent } from '@components/checked-cell-renderer/checked-cell-renderer.component';
import { ActionCellRendererComponent } from '@components/action-cell-renderer/action-cell-renderer.component';
import { apiUrl, bind } from '@utils';
import { CrudGrid, GridRequestType } from '@directives/crud-grid';
import { CreateUserComponent } from '../create-user/create-user.component';
import { EditUserComponent } from '../edit-user/edit-user.component';
import { ApiResult } from '@typings';
import { MatDialogConfig } from '@angular/material/dialog';

@Component({
  selector: 'fd-users',
  imports: [...GridImports],
  providers: [UsersService],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent extends CrudGrid<Users, UsersFilter> {

  protected override deleteUrl: string = '';
  protected override editComponent: Type<any> = EditUserComponent;
  protected override createComponent: Type<any> = CreateUserComponent;

  override columnDefs: ColDef<Users, any>[] = [
    { headerName: "ردیف", valueGetter: firstCellValueGetter(), sortable: false },
    { headerName: 'شناسه', field: 'id' },
    { headerName: 'نام', field: 'fullName' },
    { headerName: 'نام کاربری', field: 'userName' },
    { headerName: 'موبایل', field: 'phoneNumber' },
    { headerName: 'ایمیل', field: 'email' },
    { headerName: 'ورود دو عاملی', field: 'twoFactorAuthentication', cellRenderer: CheckedCellRendererComponent },
    { headerName: 'فعال', field: 'isActive', cellRenderer: CheckedCellRendererComponent },
    {
      headerName: 'عملیات',
      cellRenderer: ActionCellRendererComponent,
      cellRendererParams: defineActionCellParams(this.createActionCommands)
    }
  ];

  constructor(service: UsersService) {
    super(service);
  }

  protected override buildFilter(builder: FormBuilder): FormGroup {
    return builder.group({
      fullName: [],
      userName: [],
      phoneNumber: [],
      email: [],
      twoFactorAuthentication: [],
      isActive: []
    });
  }

  protected override configDialog(type: GridRequestType, config: MatDialogConfig): void {
    super.configDialog(type, config);
    //config.maxHeight = '95%';
    config.position = { top: '20px' };
  }

  @bind()
  private createActionCommands(): IActionCommand[] {
    const actions: IActionCommand[] = [
      {
        tooltip: 'ویرایش',
        icon: 'pa pa-edit',
        action: this.edit
      },
      {
        tooltip: 'بازنشانی رمزعبور',
        icon: 'pa pa-lock',
        action: this.resetPassword
      },
      {
        tooltip: 'فعال/غیرفعال',
        icon: 'pa pa-failed',
        action: this.activation
      }
    ];

    return actions;
  }

  @bind()
  private activation(params: ICellRendererParams<Users>): void {
    const actionName = params.data?.isActive ? 'غیرفعال سازی' : 'فعالسازی';
    const content = params.data?.isActive ? 'کاربر غیرفعال خواهد شد، آیا اطمینان دارید؟' : 'کاربر فعال خواهد شد، آیا اطمینان دارید؟';

    this.modal.open({
      title: `${actionName} کاربر`,
      content: content,
      onConfirm: (result) => {
        if (result === 'ok') {
          this.httpClient
            .post<ApiResult>(apiUrl('User/Activation'), { userId: params.data!.id })
            .subscribe(response => {
              if (response.successed) {
                this.growl.success({ content: `${actionName} کاربر با موفقیت انجام شد.` });
              }
            });
        }
        return true;
      }
    });
  }

  @bind()
  private resetPassword(params: ICellRendererParams<Users>): void {
    this.modal.open({
      title: 'بازنشانی رمز عبور',
      content: 'رمزعبور کاربر بازنشانی و برای او پیامک خواهد شد، مطمئن هستید؟',
      onConfirm: (result) => {
        if (result === 'ok') {
          this.httpClient
            .post<ApiResult>(apiUrl('User/ForceResetPassword'), { userId: params.data!.id })
            .subscribe(response => {
              if (response.successed) {
                this.growl.success({ content: 'رمزعبور کاربر بازنشانی و از طریق پیامک برای او ارسال شد.' });
              }
            });
        }
        return true;
      }
    });
  }

}
