import { Component, inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { DialogFormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { ApiService } from '@services/api.service';
import { Nullable, SelectOption } from '@typings';
import { apiUrl } from '@utils';
import { Observable } from 'rxjs';
import { Users } from '../../membership-models';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'fd-edit-user',
  imports: [...GlobalImports],
  templateUrl: './edit-user.component.html',
  styleUrl: './edit-user.component.scss'
})
export class EditUserComponent extends DialogFormBase<EditUser> {

  entity!: Users;
  roles: Observable<ReadonlyArray<SelectOption>>;

  constructor(apiService: ApiService) {
    super();

    if (!this.entity) {
      this.growl.error({ content: 'کاربر پیدا نشد.' });
      this.close();
    }

    this.roles = apiService.roles;

    this.setLoader(true);

    this.getData<EditUser>(apiUrl('User/Get'), { id: this.entity.id })
      .subscribe(response => {
        if (response.successed) {
          this.formGroup.patchValue({
            fullName: response.data.fullName,
            userName: response.data.userName,
            phoneNumber: response.data.phoneNumber,
            email: response.data.email,
            twoFactorAuthentication: response.data.twoFactorAuthentication,
            ipAddresses: response.data.ipAddresses,
            roles: response.data.roles
          });
        }
        else {
          this.close();
        }
        this.setLoader(false);
      });
  }

  protected override submitCore(): void {
    this.postData(apiUrl('User/Update'))
      .subscribe(response => {
        if (response.successed) {
          this.growl.success({ content: 'عملیات ویرایش کاربر با موفقیت انجام شد.' });
          this.close();
        }
      });
  }

  protected override buildForm(builder: FormBuilder): FormGroupOf<EditUser> {
    this.entity = inject(MAT_DIALOG_DATA);

    return builder.group({
      id: formControl<Nullable<number>>('شناسه کاربر', this.entity.id, context => context.required()),
      fullName: formControl<Nullable<string>>('نام', null, context => context.required()),
      userName: formControl<Nullable<string>>('نام کاربری', null, context => context.required()),
      phoneNumber: formControl<Nullable<string>>('شماره موبایل', null, context => context.required()),
      email: formControl<Nullable<string>>('ایمیل', null, context => context.required()),
      twoFactorAuthentication: formControl('ورود دو عاملی', false, context => context.required()),
      ipAddresses: formControl<Nullable<string>>('آدرس‌های IP', null),
      roles: formControl<number[]>('نقش‌ها', []),
    });
  }

}

interface EditUser {
  id: number;
  fullName: string;
  userName: string;
  phoneNumber: string;
  email: string;
  twoFactorAuthentication: boolean;
  ipAddresses: string;
  roles: number[];
}
