import {
  Component,
  ElementRef,
  forwardRef,
  OnDestroy,
  Input,
  ViewEncapsulation,
  ChangeDetectorRef,
  ViewChild,
  input,
} from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormInputBase } from '@directives/form-input-base.directive';
import { CalendarComponent } from '../calendar/calendar.component';
import {
  ComponentHost,
  ComponentHostFactory,
} from '@services/component-host-factory.service';
import { DateOnly } from '@components/calendar/xobin-date-adapter';
import { formatDateTime, parseDateString } from '@utils';
import { CultureInfo } from '@xobin/globalization';

@Component({
  selector: 'date-input',
  imports: [],
  templateUrl: './date-input.component.html',
  styleUrl: './date-input.component.scss',
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      multi: true,
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DateInputComponent),
    },
  ],
})
export class DateInputComponent extends FormInputBase implements OnDestroy {
  @Input()
  format: string = 'yyyy/MM/dd';

  @Input()
  minWidth = '260px';

  @Input()
  readonly?: boolean;

  @Input()
  minDate: Date | null = null;

  @ViewChild('formInput')
  _formInput!: ElementRef<HTMLInputElement>;

  private _componentHost?: ComponentHost<CalendarComponent>;
  private readonly _clickOutsideHandler = this.clickOutsideHandler.bind(this);

  constructor(
    elementRef: ElementRef<HTMLElement>,
    private readonly _cdr: ChangeDetectorRef,
    private readonly _hostFactory: ComponentHostFactory,
  ) {
    super(elementRef);
  }

  override ngAfterViewInit(): void {
    super.ngAfterViewInit();
    this.setInputValue(this._value);
  }

  override writeValue(value: any): void {
    if (value && !(value instanceof Date)) {
      throw new Error(`Invalid Date value: [${value}]`);
    }

    super.writeValue(value);
    this._cdr.markForCheck();
    this.setInputValue(value);
  }

  override _onInput(ev: Event): void {
    super._onInput(ev);
    this._componentHost?.hide();
  }

  _clear(): void {
    this._value = null;
    this.onChange(null);
    this.setInputValue(null);
    this._cdr.markForCheck();
  }

  _onFocus(): void {
    if (!this._formInput.nativeElement.readOnly) {
      this.showPicker();
    }
  }

  ngOnDestroy(): void {
    this._componentHost?.destroy();
  }

  private showPicker(): void {
    if (!this._componentHost) {
      this._componentHost = this._hostFactory.create({
        suppressMinWidth: true,
        containerCssClass: 'date-picker',
        componentType: CalendarComponent,
        target: this._elementRef.nativeElement,
        configure: (component) => {
          component.minDate = this.minDate
            ? DateOnly.fromDate(this.minDate)
            : null;
          component.selectedChange.subscribe((value) => {
            this.hidePicker();
            this._value = value;
            this.onChange(value);
            this.setInputValue(value);
            this._cdr.markForCheck();
          });
        },
      });

      this._componentHost.deactivated.subscribe(() =>
        this._componentHost!.detachHandler('click', this._clickOutsideHandler),
      );
    }

    if (!this._componentHost.isActive) {
      this._componentHost.component.value = this.getValueAsDateOnly();
      this._componentHost.component.minDate = this.minDate
        ? DateOnly.fromDate(this.minDate)
        : null;
      this._componentHost.attachHandler('click', this._clickOutsideHandler);
      this._componentHost.show();
    }
  }

  private hidePicker(): void {
    if (this._componentHost) {
      this._componentHost.hide();
      this._componentHost.detachHandler('click', this._clickOutsideHandler);
    }
  }

  private clickOutsideHandler(event: MouseEvent): void {
    if (
      !this._componentHost!.container.contains(event.target as Node) &&
      !this._elementRef.nativeElement.contains(event.target as Node)
    ) {
      this.hidePicker();
    }
  }

  private getValueAsDateOnly(): DateOnly | null {
    if (!this._value) {
      return null;
    }

    if (this._value instanceof Date) {
      return DateOnly.fromDate(this._value);
    } else if (typeof this._value == 'string') {
      try {
        const date = parseDateString(
          this._value,
          this.format,
          CultureInfo.currentCulture,
        );
        if (date) {
          this._value = date;
          return DateOnly.fromDate(date);
        }
      } catch (e) {}
    }

    return null;
  }

  private setInputValue(value: any): void {
    if (this._formInput) {
      this._formInput.nativeElement.value = formatDateTime(value, this.format);
    }
  }
}
