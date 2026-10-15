import { Directive, ElementRef, Input, Renderer2 } from '@angular/core';

@Directive({
  selector: '[negative]',
  standalone: true
})
export class NegativeDirective {

  constructor(private readonly renderer: Renderer2, private readonly ref: ElementRef<HTMLElement>) {
  }

  @Input('negative')
  set negative(value: number | undefined | null) {
    const isNegative = typeof value === 'number' && value < 0;
    const action = isNegative ? this.renderer.addClass : this.renderer.removeClass;
    action.call(this.renderer, this.ref.nativeElement, 'negative');
  }
}
