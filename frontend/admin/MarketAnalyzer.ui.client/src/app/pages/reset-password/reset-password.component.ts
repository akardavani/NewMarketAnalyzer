import { Component, inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { FormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { BrandService } from '@services/brand.service';
import { Nullable } from '@typings';
import { getQueryValue } from '@utils';

@Component({
  selector: 'fd-reset-password',
  imports: [...GlobalImports],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss',
  host: {
    'class': 'external-page'
  }
})
export class ResetPasswordComponent extends FormBase<ResetPassword> {

  companyLogo?: string;
  version = appSettings.version;
  readonly brand = inject(BrandService);

  constructor(private readonly router: Router) {
    super();
  }

  protected override submitCore(): void {
    this.postData('api/Account/ResetPassword')
      .subscribe(response => {
        if (response.successed) {
          this.router.navigateByUrl('login');
        }
      });
  }

  protected override buildForm(builder: FormBuilder): FormGroupOf<ResetPassword> {
    const username = getQueryValue('username');

    return builder.group({
      userName: formControl<Nullable<string>>('نام کاربری', username, context => context.required()),
      password: formControl<Nullable<string>>('رمز عبور جدید', null, context => context.required()),
      confirmPassword: formControl<Nullable<string>>('تکرار رمز عبور', null, context => context.required()),
      verificationCode: formControl<Nullable<string>>('کد اعتبارسنجی', null, context => context.required()),
      captchaCode: formControl<Nullable<string>>('کد امنیتی', null, context => context.required()),
    });
  }

}

export interface ResetPassword {
  userName: string;
  password: string;
  confirmPassword: string;
  verificationCode: string;
  captchaCode: string;
}
