import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { ModalButton } from '@services/modal.service';

@Component({
  selector: 'fd-modal',
  standalone: true,
  imports: [],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.scss',
  encapsulation: ViewEncapsulation.None,
  host: {
    'class': 'ng-modal-wrapper',
    '[class.align-top]': 'position == "top"',
    '[class.align-center]': 'position == "center"',
    '[class.align-bottom]': 'position == "bottom"',
    '(click)': 'backdropClicked.emit()'
  }
})
export class ModalComponent {

  @Input()
  title?: string;

  @Input()
  position: ('top' | 'center' | 'bottom') = 'top';

  @Input()
  content?: string;

  @Input()
  buttons: ModalButton[] = [];

  @Output()
  closeClicked = new EventEmitter<void>();

  @Output()
  backdropClicked = new EventEmitter<void>();

  @Output()
  buttonClicked = new EventEmitter<any>();

}
