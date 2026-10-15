import { AfterViewInit, Directive, ElementRef, inject, Input } from '@angular/core';
import { ControlValueAccessor } from '@angular/forms';
import { ControlValueAccessorObserver, VALUE_ACCESSOR_OBSERVER } from '@form-utils';
import { getValue, setValue } from '@utils';

@Directive({ standalone: true })
export abstract class FormInputBase implements AfterViewInit, ControlValueAccessor {

  _onTouched = () => { };
  onChange = (value: any) => {
    this.notifyObservers(value);
  };

  protected _value!: any;
  protected _isInitialized: boolean = false;
  protected readonly _observers: ReadonlyArray<ControlValueAccessorObserver>;

  constructor(protected readonly _elementRef: ElementRef<HTMLElement>) {
    this._observers = inject(VALUE_ACCESSOR_OBSERVER, { host: true, optional: true }) || [];
  }

  @Input()
  get disabled(): boolean {
    return getValue(this, 'disabled') || false;
  }
  set disabled(value: boolean) {
    if (this.disabled !== value) {
      setValue(this, 'disabled', value);
    }
  }

  ngAfterViewInit(): void {
    this._isInitialized = true;
  }

  writeValue(value: any): void {
    this._value = value;
    this.notifyObservers(value);
  }

  registerOnChange(fn: any): void {
    this.onChange = (value) => {
      fn(value);
      this.notifyObservers(value);
    };
  }

  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }

  setDisabledState(disabled: boolean): void {
    this.disabled = disabled;
  }

  /** Handles input events on the input. */
  _onInput(ev: Event): void {
    const value = (ev.target as HTMLInputElement).value;
    this._value = value;
    this.onChange(value);
  }

  /** Handles blur events on the input. */
  _onBlur(): void {
    this._onTouched();
  }

  protected notifyObservers(value: any): void {
    const transformedValue = this.transformValue(value);
    this._observers.forEach(observer => observer.valueChanged(transformedValue));
  }

  protected transformValue(value: any): any {
    return value;
  }

}
