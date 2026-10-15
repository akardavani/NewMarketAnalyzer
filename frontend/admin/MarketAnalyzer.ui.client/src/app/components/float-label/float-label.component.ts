import { ChangeDetectorRef, Component, Input, signal } from '@angular/core';
import { ControlValueAccessorObserver, VALUE_ACCESSOR_OBSERVER } from '@form-utils';

@Component({
  selector: 'fd-float-label',
  imports: [],
  templateUrl: './float-label.component.html',
  styleUrl: './float-label.component.scss',
  providers: [
    {
      multi: true,
      provide: VALUE_ACCESSOR_OBSERVER,
      useExisting: FloatLabelComponent
    }
  ],
  host: {
    'class': 'floating-label',
    '[class.filled]': 'floated || _filled'
  }
})
export class FloatLabelComponent implements ControlValueAccessorObserver {

  @Input() floated = false;
  @Input() checkLength = false;
  @Input() placeholder!: string;

  _filled = false;

  constructor(private readonly cdr: ChangeDetectorRef) {

  }

  valueChanged(value: any): void {
    if (this.floated) {
      this.cdr.markForCheck();
      return;
    }

    if (value == null) {
      value = '';
    }

    if (this.checkLength && isArrayLike(value)) {
      this._filled = !!value.length;
    }
    else {
      this._filled = value !== '';
    }

    this.cdr.markForCheck();
  }
}

function isArrayLike(obj: any): obj is { length: number } {
  return obj && typeof (obj['length']) == 'number';
}
