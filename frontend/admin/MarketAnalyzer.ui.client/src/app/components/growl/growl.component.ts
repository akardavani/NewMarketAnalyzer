import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  Output,
  ViewEncapsulation
} from '@angular/core';

@Component({
  selector: 'fd-growl',
  imports: [],
  templateUrl: './growl.component.html',
  styleUrl: './growl.component.scss',
  encapsulation: ViewEncapsulation.None,
  host: {
    'class': 'growl',
    '[class]': 'cssClass',
    '[class.left]': "position=='left'",
    '[class.right]': "position=='right'"
  }
})
export class GrowlComponent implements AfterViewInit {

  @Input() icon?: string;
  @Input() title?: string;
  @Input() content!: string;
  @Input() timeout?: number;
  @Input() cssClass?: string;
  @Input() position!: 'right' | 'left';

  @Output()
  readonly closed = new EventEmitter<void>();

  private _timer?: number;

  constructor(public readonly host: ElementRef<HTMLElement>) {

  }

  ngAfterViewInit(): void {
    this.hide();
  }

  @HostListener('mouseenter')
  mouseEnter(): void {
    clearTimeout(this._timer);
  }

  @HostListener('mouseleave')
  mouseLeave(): void {
    this.hide();
  }

  close(): void {
    this.closed.emit();
  }

  private hide(): void {
    this._timer = window.setTimeout(this.close.bind(this), this.timeout);
  }

}
