import { Component, ElementRef, Inject, InjectionToken, OnDestroy, forwardRef } from '@angular/core';
import { FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormInputBase } from '@directives/form-input-base.directive';
import { InputValidationDirective } from '@directives/input-validation.directive';
import { bind } from '@utils';

export interface CaptchaOptions {
  url: string;
  lifetime: number;
}

// تعریف توکن اینجا انجام می‌شود
export const CAPTCHA_OPTIONS = new InjectionToken<CaptchaOptions>('CAPTCHA_OPTIONS');

@Component({
  selector: 'fd-captcha',
  imports: [InputValidationDirective, FormsModule],
  templateUrl: './captcha.component.html',
  styleUrl: './captcha.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: forwardRef(() => CaptchaComponent)
    }
  ]
})

export class CaptchaComponent extends FormInputBase implements OnDestroy {

  _src!: string;
  _captchaTimer: number | undefined;

  constructor(
    elementRef: ElementRef<HTMLElement>,
    @Inject(CAPTCHA_OPTIONS) private readonly _options: CaptchaOptions) {

    super(elementRef);
    this.setSrc();

  }

  override ngAfterViewInit(): void {
    super.ngAfterViewInit();
    this.startCaptchaTimer();
  }

  @bind()
  refreshCaptcha(): void {
    clearTimeout(this._captchaTimer);
    this.onChange(null);
    this.setSrc();
    this.startCaptchaTimer();
  }

  ngOnDestroy(): void {
    clearTimeout(this._captchaTimer);
  }

  private setSrc(): void {
    this._src = `${this._options.url}?_d=${Date.now()}`;
  }

  private startCaptchaTimer(): void {
    this._captchaTimer = window.setTimeout(this.refreshCaptcha, this._options.lifetime);
  }

}

