import { AfterViewInit, Directive, ElementRef, Host, Renderer2 } from '@angular/core';
import { ControlContainer, FormGroupDirective, FormGroupName, NgControl } from '@angular/forms';

@Directive({
  standalone: true,
  selector: 'input[formControlName], select[formControlName], textarea[formControlName], [inputValidation]',
})
export class InputValidationDirective implements AfterViewInit {

  constructor(
    private readonly _control: NgControl,
    private readonly _renderer: Renderer2,
    private readonly _container: ControlContainer,
    @Host() private readonly _input: ElementRef<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) {

  }

  get submitted(): boolean {
    if (this._container instanceof FormGroupDirective) {
      return this._container.submitted;
    }
    else if (this._container instanceof FormGroupName) {
      return true;
    }
    else {
      return true;
    }
  }

  ngAfterViewInit(): void {
    if (this._container instanceof FormGroupDirective) {
      this._container.ngSubmit.subscribe(() => this.validate());
    }

    //if (this._container instanceof FormGroupName) {

    //}

    this._control.valueChanges?.subscribe(() => this.validate());
  }

  private validate(): void {
    const method = (this.submitted && this._control.invalid) || (!this._container && this._control.invalid && this._control.touched) ?
      'addClass' :
      'removeClass';
    this._renderer[method](this._input.nativeElement, 'input-invalid');
  }

}

//https://stackoverflow.com/questions/72549813/angular-custom-form-control
//https://stackoverflow.com/questions/59513861/access-formcontrol-inside-custom-angular-component
//https://stackoverflow.com/questions/52893088/forwarding-formcontrolname-to-inner-component-in-angular-with-reactive-forms
