import { Renderer2 } from '@angular/core';
import { Directive, ElementRef, OnDestroy, AfterViewInit, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { getValue, setValue } from '@utils';
import AutoNumeric, { Options as AutoNumericOptions } from 'autonumeric';

@Directive({
  selector: '[ngAutoNumeric]',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: AutoNumericDirective
    }
  ],
  standalone: true
})
export class AutoNumericDirective implements AfterViewInit, ControlValueAccessor, OnDestroy {

  @Input()
  nullOnEmpty: boolean = true;

  @Input()
  tick!: number;

  private _onChange: any;
  private _onTouched: any;
  private _initialValue: any;
  private _plugin: AutoNumeric | undefined;

  constructor(
    private readonly renderer: Renderer2,
    private readonly input: ElementRef<HTMLInputElement>) {

  }

  @Input("ngAutoNumeric")
  get options(): AutoNumericOptions {
    return getValue(this, 'options') || {
      allowDecimalPadding: false,
      modifyValueOnWheel: false,
      showWarnings: false,
      onInvalidPaste: "ignore",
      minimumValue: "0",
      maximumValue: String(Number.MAX_SAFE_INTEGER),
      watchExternalChanges: true,
      emptyInputBehavior: this.nullOnEmpty ? 'null' : 'min'
    };
  }
  set options(value: AutoNumericOptions | null) {
    setValue(this, 'options', Object.assign(this.options, value));
    if (this._plugin) {
      this._plugin.update(this.options);
    }
  }

  ngAfterViewInit(): void {
    //Set default tick size
    this.tick = this.tick || 1;

    this._plugin = new AutoNumeric(this.input.nativeElement, this._initialValue, this.options);
    const input = this._plugin.node() as HTMLInputElement;

    if (!input.readOnly) {
      input.onkeydown = (event) => {
        const value = this._plugin!.getNumber()!;
        // up-arrow
        if (event.key == 'ArrowUp') {
          event.preventDefault();

          const newValue = value + this.tick;
          if (newValue <= Number(this.options.maximumValue))
            this._plugin!.set(newValue);
        }
        // down-arrow
        else if (event.key == "ArrowDown") {
          event.preventDefault();

          const newValue = value - this.tick;
          if (newValue >= Number(this.options.minimumValue)) {
            this._plugin!.set(newValue);
          }
        }
      };
    }

    this.renderer.listen(input, 'autoNumeric:rawValueModified', (ev: CustomEvent) => {
      if (this._onChange) {
        this._onChange(ev.detail.newRawValue);
      }
    });
  }

  writeValue(value: any): void {
    if (this._plugin) {
      this._plugin.set(value);
    }
    else {
      this._initialValue = value;
    }
  }

  registerOnChange(fn: any): void {
    this._onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }

  ngOnDestroy(): void {
    this._plugin?.detach();
  }

}
