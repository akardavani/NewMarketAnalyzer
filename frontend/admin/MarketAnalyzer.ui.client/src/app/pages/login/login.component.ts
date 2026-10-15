import { Component, inject, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { CaptchaComponent } from '@components/captcha/captcha.component';
import { FormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { BrandService } from '@services/brand.service';
import { AuthService } from '@services/auth.service';
import { AuthResult, LoginType, Nullable } from '@typings';
import { bind } from '@utils';

@Component({
  selector: 'fd-login',
  imports: [...GlobalImports],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  host: {
    'class': 'external-page'
  }
})
export class LoginComponent extends FormBase<LoginRequest> {

  companyLogo?: string;
  version = appSettings.version;
  readonly brand = inject(BrandService);
  requiredTfa: boolean = false;

  @ViewChild(CaptchaComponent)
  captcha!: CaptchaComponent;

  private _tfaLifetime = 0;

  constructor(private readonly authService: AuthService) {
    super();
  }

  protected override submitCore(): void {
    this.loading = true;

    this.postData<AuthResult>('api/Account/Login')
      .subscribe(response => {
        if (response.successed) {
          if (response.data.requiredTfa) {
            this.captcha.refreshCaptcha();

            this.requiredTfa = true;
            this._tfaLifetime = response.data.lifetime;
            //this.startTimer(); //TODO: 
            this.setControlValue('loginType', 2);
            this.setControlValue('password', null);
          }
          else {
            this.authService.signIn(response.data);
          }
          this.endLoading();
        }
        else {
          this.captcha.refreshCaptcha();
          this.endLoading();
        }
      });
  }

  @bind()
  private endLoading(): void {
    this.loading = false;
    this._cdr.markForCheck();
  }

  protected override buildForm(builder: FormBuilder): FormGroupOf<LoginRequest> {
    return builder.group({
      userName: formControl<Nullable<string>>('نام کاربری', null, context => context.required()),
      password: formControl<Nullable<string>>('رمز عبور', null, context => context.required()),
      captchaCode: formControl<Nullable<string>>('کد امنیتی', null, context => context.required()),
      loginType: formControl<LoginType>('نوع ورود', LoginType.StaticPassword, context => context.required())
    });
  }

}

interface LoginRequest {
  userName: string;
  password: string;
  captchaCode: string;
  loginType: LoginType;
}
