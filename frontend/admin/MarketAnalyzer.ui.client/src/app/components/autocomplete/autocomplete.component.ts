import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Component, ElementRef, EventEmitter, forwardRef, Input, OnChanges, OnDestroy, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ReactiveFormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ApiResult, Nullable, PlainObject, SelectOption } from '@typings';
import { FormInputBase } from '@directives/form-input-base.directive';
import { ComponentHost, ComponentHostFactory } from '@services/component-host-factory.service';
import { debounceTime, distinctUntilChanged, fromEvent, map } from 'rxjs';
import { SelectListComponent } from '@components/select-list/select-list.component';
import { bind, buildUrl, getValue, setValue } from '@utils';

@Component({
  selector: 'fd-autocomplete',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './autocomplete.component.html',
  styleUrl: './autocomplete.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: forwardRef(() => AutocompleteComponent)
    }
  ]
})
export class AutocompleteComponent<TKey = number> extends FormInputBase implements OnChanges, OnDestroy {

  @Output()
  readonly selected = new EventEmitter<SelectOption<TKey>>();

  @ViewChild('searchInput')
  _searchInput!: ElementRef<HTMLInputElement>;

  @Input()
  initOption: Nullable<SelectOption<TKey>> = null;

  _isLoading = false;
  private _selectedOption: Nullable<SelectOption<TKey>> = null;
  private _componentHost?: ComponentHost<SelectListComponent<SelectOption<TKey>>>;

  constructor(
    elementRef: ElementRef<HTMLElement>,
    private readonly _cdr: ChangeDetectorRef,
    private readonly _httpClient: HttpClient,
    private readonly _hostFactory: ComponentHostFactory) {

    super(elementRef);

  }

  ngOnChanges(changes: SimpleChanges): void {
    const initOption = changes['initOption'];
    if (initOption) {
      this.updateValue(initOption.currentValue);
      this.notifyObservers(initOption.currentValue?.value);
      this._cdr.markForCheck();
    }
  }

  @Input()
  get dataSource(): AutocompleteDataSource<TKey> | null | undefined {
    return getValue(this, 'dataSource');
  }
  set dataSource(value) {
    if (this.dataSource != value) {
      if (isRemoteDataSource<TKey>(value) && !value.transform) {
        value = Object.assign({
          transform: (response: ApiResult<SelectOption<TKey>>) => response.successed ? response.data : []
        }, value);
      }

      setValue(this, 'dataSource', value);
      this._cdr.markForCheck();
    }
  }

  override ngAfterViewInit(): void {
    super.ngAfterViewInit();

    if (this.initOption) {
      this.updateValue(this.initOption);
      this.notifyObservers(this.initOption.value);
    }

    fromEvent(this._searchInput.nativeElement, 'input')
      .pipe(
        debounceTime(250),
        distinctUntilChanged(),
        map(() => this._searchInput.nativeElement.value)
      )
      .subscribe(this.filter);
  }

  override writeValue(value: any): void {
    if (!value) {
      this._selectedOption = null;
      this.updateValue(this._selectedOption);
      this.onChange(null);
    }
  }

  override _onBlur(): void {
    super._onBlur();
    this.updateValue(this._selectedOption);
  }

  _clear(): void {
    this.updateValue(null);
    this.hideSuggestions();
    this.onChange(null);
  }

  ngOnDestroy(): void {
    this._componentHost?.destroy();
  }

  @bind()
  private filter(keyword: string | null): void {
    if (!this.dataSource) {
      return;
    }

    if (!keyword) {
      this.hideSuggestions();
      return;
    }

    const dataSource = Object.assign({}, this.dataSource);
    if (isRemoteDataSource(dataSource)) {
      if (keyword.length < (dataSource.minChars || 2)) {
        return;
      }

      const query = Object.assign({
        [dataSource.query || 'keyword']: keyword
      }, dataSource.params);

      this._httpClient
        .get<any>(buildUrl(dataSource.url, query))
        .subscribe(response => {
          const options = dataSource.transform ?
            dataSource.transform(response) :
            response;

          this.showSuggestions(options);
        });
    }
    else if (isLocalDataSource(dataSource)) {
      const options = dataSource.options.filter(e => e.text.includes(keyword));
      this.showSuggestions(options);
    }
  }

  private showSuggestions(options: SelectOption<TKey>[]): void {
    if (!this._componentHost) {
      this._componentHost = this._hostFactory.create({
        componentType: SelectListComponent<SelectOption<TKey>>,
        target: this._searchInput.nativeElement,
        configure: component => {
          component.search = false;
          component.selection = 'single';
          component.changed.subscribe((value: SelectOption<TKey>) => {
            this.hideSuggestions();
            this.updateValue(value);
            this.onChange(value.value);
            this.selected.emit(value);
          });
        }
      });

      this._componentHost.deactivated.subscribe(() => this._componentHost!.detachHandler('click', this.clickOutsideHandler))
    }

    this._componentHost.component.options = options;
    this._componentHost.attachHandler('click', this.clickOutsideHandler);
    this._componentHost.component.attachSearchBox(this._searchInput.nativeElement);
    this._componentHost.show();
  }

  private hideSuggestions(): void {
    if (this._componentHost) {
      this._componentHost.hide();
      this._componentHost.detachHandler('click', this.clickOutsideHandler);
      this._componentHost.component.detachSearchBox(this._searchInput.nativeElement);
    }
  }

  private updateValue(option: Nullable<SelectOption<TKey>>) {
    this._selectedOption = option;
    if (this._searchInput) {
      this._searchInput.nativeElement.value = option?.text || '';
    }
  }

  @bind()
  private clickOutsideHandler(event: MouseEvent): void {
    if (!this._componentHost!.container.contains(event.target as Node) &&
      !this._elementRef.nativeElement.contains(event.target as Node)) {
      this.hideSuggestions();
    }
  }

}

export type RemoteDataSource<TKey = number> = {
  url: string;
  query?: string;
  minChars?: number;
  params?: PlainObject;
  transform?: (response: any) => ReadonlyArray<SelectOption<TKey>>;
};

export type LocalDataSource<TKey> = { options: SelectOption<TKey>[] };
export type AutocompleteDataSource<TKey> = RemoteDataSource<TKey> | LocalDataSource<TKey>;

function isRemoteDataSource<TKey>(obj: any): obj is RemoteDataSource<TKey> {
  return obj && typeof (obj['url']) === 'string';
}

function isLocalDataSource<TKey>(obj: any): obj is LocalDataSource<TKey> {
  return obj && typeof Array.isArray((obj['options']));
}
