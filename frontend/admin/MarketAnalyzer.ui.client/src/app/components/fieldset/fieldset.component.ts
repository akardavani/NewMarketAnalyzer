import { Component, Input, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'fd-fieldset',
  imports: [],
  templateUrl: './fieldset.component.html',
  styleUrl: './fieldset.component.scss',
  encapsulation: ViewEncapsulation.None,
  host: {
    'class': 'fieldset'
  }
})
export class FieldsetComponent {

  @Input()
  caption: string = '';

}
