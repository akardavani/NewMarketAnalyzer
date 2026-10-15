import { Directive, ElementRef, Host, Input, Renderer2 } from '@angular/core';
import { FormGroupDirective } from '@angular/forms';
import { Subscription } from 'rxjs';

@Directive({
  selector: '[errorFor]',
  standalone: true
})
export class ErrorMessageDirective {

  @Input('errorFor')
  controlName!: string;

  private _sub!: Subscription;

  constructor(
    private readonly _renderer: Renderer2,
    @Host() private readonly _parent: FormGroupDirective,
    @Host() private readonly _element: ElementRef<HTMLElement>) {

  }

  ngAfterViewInit(): void {
    const control = this._parent.form.get(this.controlName);
    if (!control) {
      throw new Error(`The form not hav any control name: ${this.controlName}`);
    }

    this._sub = control
      .valueChanges
      .subscribe(() => {
        if (!!control.errors && control.invalid && (control.dirty || control.touched)) {
          const keys = Object.keys(control.errors);
          this.validate(keys.length ? control.getError(keys[0]) : '');
        }
        else {
          this.validate('');
        }
      });

    this._parent
      .ngSubmit
      .subscribe(() => {
        if (!!control.errors && control.invalid) {
          const keys = Object.keys(control.errors);
          this.validate(keys.length ? control.getError(keys[0]) : '');
        }
        else {
          this.validate('');
        }
      });

  }

  ngOnDestroy(): void {
    this._sub.unsubscribe();
  }

  private validate(error: string): void {
    this._element.nativeElement.innerText = error;
    const method = !!error ? 'addClass' : 'removeClass';
    this._renderer[method](this._element.nativeElement, 'invalid');
  }

}
