import { Component, Input, Pipe, PipeTransform, ViewEncapsulation } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({ name: "highlight" })
class HighlightPipe implements PipeTransform {

  constructor(private readonly _sanitizer: DomSanitizer) {
  }

  transform(value: string, keyword: string | null) {
    if (!keyword) {
      return value;
    }

    const index = value.toLocaleLowerCase()
      .indexOf(keyword.toLocaleLowerCase());

    if (index < 0) {
      return value;
    }

    const highlighted = `${value.slice(0, index)}<strong class="highlight">${value.slice(index, index + keyword.length)}</strong>${value.slice(index + keyword.length)}`;

    return this._sanitizer.bypassSecurityTrustHtml(highlighted);
  }

}

@Component({
  selector: 'select-list-item',
  imports: [HighlightPipe],
  templateUrl: './select-list-item.component.html',
  encapsulation: ViewEncapsulation.None
})
export class SelectListItemComponent {

  @Input()
  caption!: string;

  @Input()
  keyword = '';
}
