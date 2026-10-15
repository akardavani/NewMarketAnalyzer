import { AfterContentChecked, AfterContentInit, Component, ContentChildren, ElementRef, forwardRef, Input, OnDestroy, QueryList, ViewChild, ViewEncapsulation } from '@angular/core';
import { NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { OptionComponent } from '@components/drop-down/option.component';
import { SelectListComponent, SelectionMode } from '@components/select-list/select-list.component';
import { FormInputBase } from '@directives/form-input-base.directive';
import { ComponentHost, ComponentHostFactory } from '@services/component-host-factory.service';
import { Nullable, SelectOption } from '@typings';
import { bind, getValue, setValue } from '@utils';

@Component({
  selector: 'fd-drop-down',
  imports: [ReactiveFormsModule],
  templateUrl: './drop-down.component.html',
  styleUrl: './drop-down.component.scss',
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      multi: true,
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DropDownComponent)
    }
  ],
  host: {
    'class': 'drop-down',
    '(click)': 'toggle()'
  }
})
export class DropDownComponent<TValue = number> extends FormInputBase implements AfterContentInit, OnDestroy {

  @Input()
  search = false;

  @ViewChild('formInput')
  _formInput!: ElementRef<HTMLInputElement>;

  @ContentChildren(OptionComponent)
  _contentOptions!: QueryList<OptionComponent>;

  private _options: SelectOption<TValue>[] = [];
  private _selectedOptions!: Nullable<SelectOption<TValue>> | SelectOption<TValue>[];
  private _componentHost?: ComponentHost<SelectListComponent<SelectOption<TValue>>>;

  constructor(
    elementRef: ElementRef<HTMLInputElement>,
    private readonly _hostFactory: ComponentHostFactory) {

    super(elementRef);

  }

  @Input()
  get selection(): SelectionMode {
    return getValue(this, 'selection') || 'single';
  }
  set selection(value: SelectionMode) {
    setValue(this, 'selection', value || 'single');
  }

  ngAfterContentInit(): void {
    this.parseOptions();

    this._contentOptions
      .changes
      .subscribe(this.parseOptions);
  }

  override ngAfterViewInit(): void {
    super.ngAfterViewInit();
    this.setLabel();
  }

  toggle(): void {
    if (this._componentHost?.isActive) {
      this.hideSuggestions();
    }
    else {
      this.showSuggestions();
    }
  }

  override writeValue(value: TValue[] | Nullable<TValue>): void {
    this._value = value;
    this.parseValue();
    this.notifyObservers(value);
  }

  ngOnDestroy(): void {
    this._componentHost?.destroy();
  }

  protected override transformValue(value: any): any {
    return this._selectedOptions;
  }

  @bind()
  private parseOptions(): void {
    this._options = this._contentOptions
      .map(item => ({
        value: item.value,
        text: item.text
      }));

    this.parseValue();
    this.notifyObservers(this._selectedOptions);
    if (this._componentHost) {
      this._componentHost.component.options = this._options;
    }
  }

  private parseValue(): void {
    const value = this._value;
    if (Array.isArray(value)) {
      this._selectedOptions = this._options.filter(e => value.includes(e.value));
    }
    else {
      this._selectedOptions = this._options.find(e => e.value == value) || null;
    }

    this.setLabel();
  }

  private setLabel(): void {
    if (this._isInitialized) {
      let text = '';
      const options = this._selectedOptions;
      if (Array.isArray(options)) {
        if (options.length == 1) {
          text = options[0].text;
        }
        else if (options.length > 1) {
          text = `${options.length} مورد انتخاب شده`;
        }
      }
      else if (options != null) {
        text = options.text;
      }

      this._formInput.nativeElement.value = text;
    }
  }

  private showSuggestions(): void {
    if (!this._componentHost) {
      this._componentHost = this._hostFactory.create({
        target: this._formInput.nativeElement,
        containerCssClass: 'drop-down-container',
        componentType: SelectListComponent<SelectOption<TValue>>,
        configure: component => {
          component.search = this.search;
          component.selectFocused = true;
          component.options = this._options;
          component.selection = this.selection;
          component.attachSearchBox(this._formInput.nativeElement);
          component.changed.subscribe(value => {
            this._value = Array.isArray(value) ?
              value.map(e => e.value) :
              value.value;

            this._selectedOptions = value;
            this.hideSuggestions();
            this._formInput.nativeElement.focus();
            this.setLabel();
            this.onChange(this._value);
          });
        }
      });

      this._componentHost.deactivated.subscribe(() => this._componentHost!.detachHandler('click', this.clickOutsideHandler));
    }

    this._componentHost.component.search = this.search;
    this._componentHost.component.selectFocused = false;
    this._componentHost.show();
    this._componentHost.attachHandler('click', this.clickOutsideHandler);
  }

  private hideSuggestions(): void {
    if (this._componentHost) {
      this._componentHost.hide();
      this._componentHost.component.selectFocused = true;
      this._componentHost.detachHandler('click', this.clickOutsideHandler);
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

