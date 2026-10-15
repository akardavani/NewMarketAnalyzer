import { AfterViewInit, Directive, ElementRef, HostListener, Input, Renderer2 } from '@angular/core';

@Directive({ selector: '[dialogAutoHeight]' })
export class DialogAutoHeightDirective implements AfterViewInit {

  @Input()
  header?: HTMLElement;

  @Input()
  footer?: HTMLElement;

  @Input()
  margin = 50;

  constructor(
    private readonly _host: ElementRef<HTMLElement>,
    private readonly renderer: Renderer2) {
  }

  ngAfterViewInit(): void {
    this.adjustHeight();
  }


  @HostListener('window:resize')
  onResize() {
    this.adjustHeight();
  }

  adjustHeight(): void {
    const headerHeight = this.header?.offsetHeight || 0;
    const footerHeight = this.footer?.offsetHeight || 0;
    const windowHeight = window.innerHeight;

    const newHeight = windowHeight - headerHeight - footerHeight - this.margin;

    this.renderer.setStyle(this._host.nativeElement, 'overflow-y', 'auto');
    this.renderer.setStyle(this._host.nativeElement, 'max-height', `${newHeight}px`);
  }

}
