import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  signal,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { SelectListItemComponent } from './select-list-item.component';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { getMemberValue, getOrCreateValue, getValue, setValue } from '@utils';

const TARGET_KEYS = ['Tab', 'ArrowDown', 'ArrowUp', 'Enter'];

@Component({
  selector: 'select-list',
  imports: [SelectListItemComponent, ReactiveFormsModule],
  templateUrl: './select-list.component.html',
  styleUrl: './select-list.component.scss',
  encapsulation: ViewEncapsulation.None,
  host: {
    'class': 'fd-select-list'
  }
})
export class SelectListComponent<TOption> implements AfterViewInit {

  @Input()
  search = true;

  @Input()
  textMember = 'text';

  @Input()
  selectFocused = false;

  @Input()
  selection: SelectionMode = 'single';

  @Output()
  readonly changed = new EventEmitter<TOption | TOption[]>();

  _keyword = signal('');
  _empty = signal(false);
  _items: SelectListItem<TOption>[] = [];
  _searchInput = new FormControl<string>('');
  _selectedItems: SelectListItem<TOption>[] = [];
  @ViewChild('focusControl') _focusControl!: ElementRef<HTMLInputElement>;

  constructor(private readonly _cdr: ChangeDetectorRef) {

  }

  @Input()
  get emptyText(): string {
    return getValue(this, 'emptyText');
  }
  set emptyText(value: string) {
    setValue(this, 'emptyText', value);
    this._cdr.markForCheck();
  }

  @Input()
  get options(): ReadonlyArray<TOption> {
    return getValue(this, 'options');
  }
  set options(value: ReadonlyArray<TOption>) {
    if (this.options == value) {
      return;
    }
    this._selectedItems = [];
    this._empty.set(!value.length);
    setValue(this, 'options', value);
    this._items = value.map(option => ({
      option: option,
      caption: getMemberValue(option, this.textMember || 'text')
    }));
    
    this._cdr.detectChanges();
  }

  ngAfterViewInit(): void {
    if (this.search) {
      this._searchInput
        .valueChanges
        .pipe(
          debounceTime(250),
          distinctUntilChanged()
        )
        .subscribe(this.filterOptions.bind(this));
    }
  }

  attachSearchBox(input: HTMLInputElement): void {
    const handler = getOrCreateValue(this, '_onKeyDown', () => this._onKeyDown.bind(this));
    input.addEventListener('keydown', handler);
  }

  detachSearchBox(input: HTMLInputElement): void {
    const handler = getValue(this, '_onKeyDown');
    input.addEventListener('keydown', handler);
  }

  _onClicked(): void {
    this._focusControl.nativeElement.focus();
  }

  _onKeyDown(event: KeyboardEvent): void {
    if (!TARGET_KEYS.includes(event.key)) {
      return;
    }

    event.preventDefault();
    const items = this._items.filter(e => !e.filtered);
    if (!items.length) {
      return;
    }

    const activeIndex = items.findIndex(e => e.focused);
    switch (event.key) {
      case 'ArrowDown':
        this._changeFocused(items, activeIndex + 1);
        break;

      case 'ArrowUp':
        this._changeFocused(items, activeIndex - 1);
        break;

      case 'Enter':
        if (activeIndex >= 0) {
          const activeItem = items[activeIndex];
          if (activeItem) {
            this._onItemSelected(activeItem);
          }
        }
        break;
    }

    this._cdr.markForCheck();
  }

  _changeFocused(items: SelectListItem<TOption>[], index: number): void {
    if (!items.length) {
      return;
    }

    this._items.forEach(e => e.focused = false);

    if (index >= items.length) {
      index = 0;
    }
    else if (index < 0) {
      index = items.length - 1;
    }

    const item = items[index];
    item.focused = true;
    if (this.selectFocused) {
      this._onItemSelected(item);
    }
  }

  _onItemSelected(item: SelectListItem<TOption>): void {
    if (this.selection == 'single') {
      const oldItem = this._selectedItems[0];
      if (oldItem == item) {
        return;
      }

      if (oldItem) {
        oldItem.focused = false;
      }

      item.focused = true;
      this._selectedItems[0] = item;
      this.changed.emit(item.option);
    }
    else {
      const index = this._selectedItems.indexOf(item);
      if (index > -1) {
        item.focused = false;
        this._selectedItems.splice(index, 1);
      }
      else {
        item.focused = true;
        this._selectedItems.push(item);
      }

      this.changed.emit(this._selectedItems.map(e => e.option));
    }
  }

  private filterOptions(value: string | null): void {
    let filteredCount = 0;

    this._items.forEach(item => {
      item.filtered = !!value && !getMemberValue(item.option, this.textMember || '').includes(value);
      if (item.filtered) {
        filteredCount++;
      }
    });

    this._keyword.set(value || '');
    this._empty.set(filteredCount == this._items.length);
  }

}

export type SelectionMode = 'single' | 'multiple';

interface SelectListItem<TOption> {
  option: TOption;
  caption: string;
  focused?: boolean;
  filtered?: boolean;
}
