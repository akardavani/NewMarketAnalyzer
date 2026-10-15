import { ElementRef, inject, OnInit } from '@angular/core';
import { Directive } from '@angular/core';
import { NgControl } from '@angular/forms';
import { ControlValueAccessorObserver, VALUE_ACCESSOR_OBSERVER } from '@form-utils';

@Directive({
  selector: 'input[formControl], input[formControlName], select[formControl], select[formControlName], textarea[formControl], textarea[formControlName]',
})
export class InputControlAccessorDirective implements OnInit {

  protected readonly _observers: ReadonlyArray<ControlValueAccessorObserver> | null;

  constructor(private readonly _control: NgControl) {
    this._observers = inject(VALUE_ACCESSOR_OBSERVER, { host: true, optional: true });
  }

  ngOnInit(): void {
    if (this._observers) {
      this._observers.forEach(observer => observer.valueChanged(this._control.value));

      this._control.valueChanges?.subscribe(value => {
        this._observers!.forEach(observer => observer.valueChanged(value));
      });
    }
  }

}

@Directive({
  selector: 'input:not([formControl]):not([formControlName]), select:not([formControl]):not([formControlName]), textarea:not([formControl]):not([formControlName])',
})
export class InputValueAccessorDirective implements OnInit {

  protected readonly _observers: ReadonlyArray<ControlValueAccessorObserver> | null;

  constructor(private readonly _ref: ElementRef<HTMLInputElement>) {
    this._observers = inject(VALUE_ACCESSOR_OBSERVER, { host: true, optional: true });
  }

  ngOnInit(): void {
    if (this._observers) {
      this._observers.forEach(observer => observer.valueChanged(this._ref.nativeElement.value));
    }
  }

}
