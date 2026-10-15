import { AfterContentInit, Component, ContentChildren, forwardRef, inject, Input, QueryList } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { CheckBoxChanged, CheckboxComponent } from '@components/checkbox/checkbox.component';
import { ControlValueAccessorObserver, VALUE_ACCESSOR_OBSERVER } from '@form-utils';
import { bind, getValue, setValue } from '@utils';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'checkbox-group',
  imports: [CheckboxComponent],
  template: `
    @if (showSelectAll) {
      <checkbox [label]="selectAllLabel" [checked]="selectAllChecked" (changed)="onSelectAllChanged($event)"></checkbox>
    }
    <ng-content></ng-content>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckboxGroupComponent),
      multi: true
    }
  ]
})
export class CheckboxGroupComponent<TValue = any> implements AfterContentInit, ControlValueAccessor {
  @ContentChildren(CheckboxComponent)
  checkboxes!: QueryList<CheckboxComponent>;
  _onTouched = () => { };
  onChange = (value: any) => {
    this.notifyObservers(value);
  };
  _value: TValue[] = [];
  protected readonly _observers: ReadonlyArray<ControlValueAccessorObserver>;

  @Input() showSelectAll = false;
  @Input() selectAllLabel = 'Select All';
  selectAllChecked = false;

  constructor() {
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

  ngAfterContentInit(): void {
    this.subscribeInputsChanges();
    this.checkboxes
      .changes
      .subscribe(this.subscribeInputsChanges);
  }

  writeValue(value: any): void {
    if (value && !(Array.isArray(value))) {
      throw new Error(`The value must array but get: ${typeof (value)}`);
    }
    this._value = value || [];
    this.notifyObservers(value);
    if (this.checkboxes) {
      this.checkboxes
        .forEach(checkbox => checkbox.checked = this._value.includes(checkbox.value))
    }
    this.updateSelectAllChecked();
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

  private _unsubscriber = new Subject<void>();

  @bind()
  private subscribeInputsChanges(): void {
    this._unsubscriber.next();
    if (!this.checkboxes) return;
    const values = this._value;
    this.checkboxes
      .forEach(checkbox => {
        checkbox.checked = values.includes(checkbox.value);
        checkbox.changed
          .pipe(takeUntil(this._unsubscriber))
          .subscribe(this.checkboxChanged);
      });
    this.updateSelectAllChecked();
  }

  @bind()
  private checkboxChanged(event: CheckBoxChanged<TValue>): void {
    const values = this._value;
    const index = values.indexOf(event.value);
    if (event.checked) {
      if (index < 0) {
        values.push(event.value);
      }
    }
    else {
      const index = values.indexOf(event.value);
      if (index >= 0) {
        values.splice(index, 1);
      }
    }
    this._value = values;
    this.onChange(this._value);
    this.updateSelectAllChecked();
  }

  onSelectAllChanged(event: CheckBoxChanged<any>): void {
    if (!this.checkboxes) return;
    const allValues = this.checkboxes.toArray().map(c => c.value).filter(v => v != null);
    if (event.checked) {
      this._value = [...allValues];
      this.checkboxes.forEach(c => c.checked = true);
    } else {
      this._value = [];
      this.checkboxes.forEach(c => c.checked = false);
    }
    this.onChange(this._value);
    this.updateSelectAllChecked();
  }

  private updateSelectAllChecked(): void {
    if (!this.showSelectAll || !this.checkboxes || this.checkboxes.length === 0) {
      this.selectAllChecked = false;
      return;
    }
    const allValues = this.checkboxes.toArray().map(c => c.value).filter(v => v != null);
    const current = this._value || [];
    this.selectAllChecked = allValues.length > 0 &&
      current.length === allValues.length &&
      current.every(v => allValues.includes(v));
  }

  private notifyObservers(value: any): void {
    this._observers.forEach(observer => observer.valueChanged(value));
  }
}
