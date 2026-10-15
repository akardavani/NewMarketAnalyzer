import { Component, inject, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { CaptchaComponent } from '@components/captcha/captcha.component';
import { FormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { BrandService } from '@services/brand.service';
import { Nullable } from '@typings';
import { apiUrl, buildUrl } from '@utils';

@Component({
  selector: 'fd-forgot-password',
  imports: [...GlobalImports],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
  host: {
    'class': 'external-page'
  }
})
export class ForgotPasswordComponent extends FormBase<ForgotPassword> {

  companyLogo?: string;
  version = appSettings.version;
  readonly brand = inject(BrandService);

  @ViewChild(CaptchaComponent)
  captcha!: CaptchaComponent;

  constructor(private readonly router: Router) {
    super();
  }

  protected override submitCore(): void {
    this.postData<number>('api/Account/ForgotPassword')
      .subscribe(response => {
        this.captcha.refreshCaptcha();

        if (response.successed) {
          this.growl.success({ content: 'کد اعتبارسنجی برای بازنشانی رمز عبور برای شما ارسال شد.' });
          this.router.navigateByUrl(buildUrl('reset-password', { username: this.formValue.userName }));
        }
      });
  }

  protected override buildForm(builder: FormBuilder): FormGroupOf<ForgotPassword> {
    return builder.group({
      userName: formControl<Nullable<string>>('نام کاربری', null, context => context.required()),
      captchaCode: formControl<Nullable<string>>('کد امنیتی', null, context => context.required()),
    });
  }

}

interface ForgotPassword {
  userName: string;
  captchaCode: string;
}
