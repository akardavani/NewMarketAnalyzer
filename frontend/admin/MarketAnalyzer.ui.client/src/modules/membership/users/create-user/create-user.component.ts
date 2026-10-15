import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { DialogFormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { ApiService } from '@services/api.service';
import { Nullable, SelectOption } from '@typings';
import { apiUrl } from '@utils';
import { Observable } from 'rxjs';

@Component({
  selector: 'fd-create-user',
  imports: [...GlobalImports],
  templateUrl: './create-user.component.html',
  styleUrl: './create-user.component.scss'
})
export class CreateUserComponent extends DialogFormBase<CreateUser> {

  roles: Observable<ReadonlyArray<SelectOption>>;

  constructor(apiService: ApiService) {
    super();
    this.roles = apiService.roles;

    this.loading = true;

  }

  protected override submitCore(): void {
    this.postData(apiUrl('User/Create'))
      .subscribe(response => {
        if (response.successed) {
          this.growl.success({ content: 'عملیات ایجاد کاربر جدید با موفقیت انجام شد.' });
          this.reset();
        }
      });
  }

  protected override buildForm(builder: FormBuilder): FormGroupOf<CreateUser> {
    return builder.group({
      fullName: formControl<Nullable<string>>('نام', null, context => context.required()),
      userName: formControl<Nullable<string>>('نام کاربری', null, context => context.required()),
      password: formControl<Nullable<string>>('رمز عبور', null, context => context.required()),
      confirmPassword: formControl<Nullable<string>>('تکرار رمز عبور', null, context => context.required()),
      phoneNumber: formControl<Nullable<string>>('شماره موبایل', null, context => context.required()),
      email: formControl<Nullable<string>>('ایمیل', null, context => context.required()),
      twoFactorAuthentication: formControl('ورود دو عاملی', false, context => context.required()),
      ipAddresses: formControl<Nullable<string>>('آدرس‌های IP', null),
      roles: formControl<number[]>('نقش‌ها', []),
    });
  }

}

interface CreateUser {
  fullName: string;
  userName: string;
  password: string;
  confirmPassword: string;
  phoneNumber: string;
  email: string;
  twoFactorAuthentication: boolean;
  ipAddresses: string;
  roles: number[];
}
