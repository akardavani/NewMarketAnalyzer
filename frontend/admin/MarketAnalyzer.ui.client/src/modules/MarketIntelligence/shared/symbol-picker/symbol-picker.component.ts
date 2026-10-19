import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StrategyApiService, SymbolItem } from '../strategy-api.service';

@Component({
  selector: 'mi-symbol-picker',
  imports: [FormsModule],
  templateUrl: './symbol-picker.component.html',
  styleUrl: './symbol-picker.component.scss'
})
export class SymbolPickerComponent {
  private readonly api = inject(StrategyApiService);
  @Input() multiple = true;
  @Input() selected: SymbolItem[] = [];
  @Output() selectedChange = new EventEmitter<SymbolItem[]>();

  text = '';
  options: SymbolItem[] = [];
  private timer?: ReturnType<typeof setTimeout>;

  onInput(): void {
    clearTimeout(this.timer);
    if (!this.text.trim()) { this.options = []; return; }
    this.timer = setTimeout(() => this.api.searchSymbols(this.text.trim()).subscribe(o => (this.options = o)), 300);
  }

  add(item: SymbolItem): void {
    const next = this.multiple ? [...this.selected.filter(x => x.symbolKey !== item.symbolKey), item] : [item];
    this.selected = next;
    this.selectedChange.emit(next);
    this.text = '';
    this.options = [];
  }

  remove(item: SymbolItem): void {
    this.selected = this.selected.filter(x => x !== item);
    this.selectedChange.emit(this.selected);
  }
}
