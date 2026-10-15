import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { DialogFormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { ApiResult, Nullable } from '@typings';
import { apiUrl } from '@utils';

@Component({
  selector: 'fd-change-password',
  imports: [...GlobalImports],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss'
})
export class ChangePasswordComponent extends DialogFormBase<ChangePassword> {

  timer = '00:00';
  _interval?: number;
  disableOtpRequest = false;

  constructor() {
    super();
  }

  requestOtp(): void {
    this.disableOtpRequest = true;
    this._cdr.markForCheck();

    this._httpClient
      .get<ApiResult<number>>(apiUrl('Account/GetChangePasswordOtpCode'))
      .subscribe(response => {
        if (response.successed) {
          this.updateTimer(response.data);
        }
        else {
          this.disableOtpRequest = false;
        }

        this._cdr.markForCheck();
      });
  }

  override ngOnDestroy(): void {
    super.ngOnDestroy();
    window.clearInterval(this._interval);
  }

  protected override submitCore(): void {
    this.postData(apiUrl('Account/ChangePassword'))
      .subscribe(response => {
        if (response.successed) {
          this.growl.success({ content: 'تغییر رمز عبور با موفقیت انجام شد.' });
          this.close();
        }
      });
  }

  protected override buildForm(builder: FormBuilder): FormGroupOf<ChangePassword> {
    return builder.group({
      verificationCode: formControl<Nullable<string>>('کد اعتبارسنجی', null, context => context.required()),
      oldPassword: formControl<Nullable<string>>('رمز عبور فعلی', null, context => context.required()),
      newPassword: formControl<Nullable<string>>('رمز عبور جدید', null, context => context.required()),
      confirmNewPassword: formControl<Nullable<string>>('تکرار رمز عبور جدید', null, context => context.required()),
    });
  }

  private updateTimer(time: number): void {
    const tiks = 60 * 1000;
    const minutes = Math.floor(time / tiks);
    const seconds = (time % tiks) / 1000;
    this.timer = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
    time -= 1000;
    if (time <= 0) {
      this.disableOtpRequest = false;
    }
    else {
      window.setTimeout(() => this.updateTimer(time), 1000);
    }

    this._cdr.markForCheck();
  }

}

interface ChangePassword {
  oldPassword: string;
  newPassword: string;
  confirmNewPassword: string;
  verificationCode: string;
}
