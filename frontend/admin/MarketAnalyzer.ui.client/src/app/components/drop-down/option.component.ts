import { AfterContentInit, Component, ElementRef, Input } from '@angular/core';

@Component({
  selector: 'fd-option',
  template: '<ng-content></ng-content>'
})
export class OptionComponent<TValue = any> implements AfterContentInit {

  @Input()
  value?: TValue;

  text: string = '';

  constructor(private readonly _ref: ElementRef<HTMLElement>) {

  }

  ngAfterContentInit(): void {
    this.text = this._ref.nativeElement.textContent?.trim() || '';
  }

}
