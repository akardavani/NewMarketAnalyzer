import { AfterViewInit, Component, ElementRef, Input, Renderer2, ViewChild } from '@angular/core';
import { AutoNumericDirective } from '@directives/auto-numeric.directive';
import { InputCellRendererParams } from '@grid-utils';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import AutoNumeric from 'autonumeric';

@Component({
  selector: 'fd-input-cell-renderer',
  imports: [AutoNumericDirective],
  templateUrl: './input-cell-renderer.component.html',
  styleUrl: './input-cell-renderer.component.scss'
})
export class InputCellRendererComponent implements AfterViewInit, ICellRendererAngularComp {


  @Input() type = 'text';

  @Input() autoNumeric: AutoNumeric.Options | null | undefined;

  @ViewChild('formInput')
  _input?: ElementRef<HTMLInputElement>;

  @ViewChild(AutoNumericDirective)
  _plugin?: AutoNumericDirective;

  params!: InputCellRendererParams<any, any>;

  _value?: string;

  constructor(private readonly renderer: Renderer2) {

  }

  ngAfterViewInit(): void {
    if (this.autoNumeric) {
      this.renderer.listen(this._input!.nativeElement, 'autoNumeric:rawValueModified', (ev: CustomEvent) => {
        if (this.params.setValue) {
          this.params.setValue(ev.detail.newRawValue);
        }
      });
    }
  }

  agInit(params: InputCellRendererParams<any, any>): void {
    this.init(params);
  }

  refresh(params: InputCellRendererParams<any, any>): boolean {
    this.init(params);
    return true;
  }

  _onChange(): void {
    if (this.params.onChange) {
      this.params.onChange(this._input?.nativeElement.value);
    }
  }

  private init(params: InputCellRendererParams<any, any>): void {
    this.params = params;
    this.type = params.type || 'text';
    this.autoNumeric = params.autoNumeric;

    const value = this.params.valueFormatted || this.params.value;

    if (this._plugin) {
      this._plugin.writeValue(value);
    }
    else if (this._input) {
      this._input.nativeElement.value = value == null ? "" : value;
    }
    else {
      this._value = value == null ? "" : value;
    }
  }

}
