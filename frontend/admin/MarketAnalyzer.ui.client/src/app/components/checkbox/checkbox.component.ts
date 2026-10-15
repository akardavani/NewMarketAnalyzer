import { ChangeDetectorRef, Component, ElementRef, EventEmitter, forwardRef, inject, Input, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ControlValueAccessorObserver, VALUE_ACCESSOR_OBSERVER } from '@form-utils';
import { getValue, setValue } from '@utils';

@Component({
  selector: 'checkbox',
  imports: [],
  templateUrl: './checkbox.component.html',
  styleUrl: './checkbox.component.scss',
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckboxComponent),
      multi: true
    }
  ],
  host: {
    'class': 'ag-checkbox'
  }
})
export class CheckboxComponent<TValue = any> implements ControlValueAccessor {

  @Input() label = '';
  @Input() value: any;
  @Input() indeterminate = false;

  @Output()
  changed = new EventEmitter<CheckBoxChanged<TValue>>();

  @ViewChild('checkbox', { static: true })
  _inputRef!: ElementRef<HTMLInputElement>;

  _onTouched = () => { };
  _onChange = (value: any) => {
    this.notifyObservers(value);
  };
  private readonly _observers: ReadonlyArray<ControlValueAccessorObserver>;

  constructor(private readonly cdr: ChangeDetectorRef) {
    this._observers = inject(VALUE_ACCESSOR_OBSERVER, { host: true, optional: true }) || [];
  }

  @Input()
  get checked(): boolean {
    return getValue(this, 'checked') || false;
  }
  set checked(value: boolean) {
    setValue(this, 'checked', !!value);
    if (this._inputRef) {
      this._inputRef.nativeElement.checked = !!value;
    }
    this.cdr.detectChanges();
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

  writeValue(value: any): void {
    this.checked = value;
    this.notifyObservers(value);
  }

  registerOnChange(fn: any): void {
    this._onChange = (value) => {
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

  onCheckboxChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this._onChange(input.checked);
    this.changed.emit({
      checked: input.checked,
      value: this.value
    });
  }

  private notifyObservers(value: any): void {
    this._observers.forEach(observer => observer.valueChanged(value));
  }

}

export interface CheckBoxChanged<TValue = any> {
  value: TValue;
  checked: boolean;
}
