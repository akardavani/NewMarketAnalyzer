import { ChangeDetectorRef, Component, ElementRef, EventEmitter, HostListener, Input, Output, ViewChild } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormInputBase } from '@directives/form-input-base.directive';
import { Func } from '@typings';
import { bind } from '@utils';

@Component({
  selector: 'file-input',
  standalone: true,
  imports: [],
  templateUrl: './file-input.component.html',
  styleUrl: './file-input.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: FileInputComponent
    }
  ],
  host: {
    class: 'form-control',
  }
})
export class FileInputComponent extends FormInputBase {

  @Input()
  placeholder: string = '';

  @Input()
  accept: string = '*/*';

  @Input()
  multiple: boolean = false;

  @Input()
  valueFormatter?: Func<(FileContent[] | null), any> = files => {
    if (!files) {
      return null;
    }

    if (this.multiple) {
      return files.length ? files.map(e => e.content) : null;
    }
    else {
      return files.length ? files[0].content : null;
    }
  };

  @Output()
  readonly changed = new EventEmitter<FileContent[] | null>();

  @ViewChild('inputFile')
  _inputFile!: ElementRef<HTMLInputElement>;

  _tooltip: string = '';
  _caption: string | null = null;

  constructor(elementRef: ElementRef<HTMLInputElement>, private readonly _cdr: ChangeDetectorRef) {
    super(elementRef);
  }

  override writeValue(value: any): void {
    super.writeValue(value);
    this.raiseChangeEvent(null);
  }

  _inputChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      if (this.multiple) {
        this.readFiles(Array.from(input.files));
      }
      else {
        this.readFile(input.files[0])
          .then(file => this.raiseChangeEvent([file]));
      }
    }
    else {
      this.raiseChangeEvent(null);
    }
  }

  @HostListener('click')
  onClick() {
    this._inputFile.nativeElement.click();
  }

  @HostListener('dragover', ['$event'])
  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  @HostListener('drop', ['$event'])
  onDrop(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer) {
      if (event.dataTransfer.files.length) {
        if (this.multiple) {
          this.readFiles(Array.from(event.dataTransfer.files));
        }
        else {
          this.readFile(event.dataTransfer.files[0])
            .then(file => this.raiseChangeEvent([file]));
        }
      }
      else if (event.dataTransfer.items.length) {
        const files = Array
          .from(
            event.dataTransfer.items,
            (item) => item.kind === 'file' ? item.getAsFile() : null)
          .filter(item => !!item);
        if (this.multiple) {
          this.readFiles(files);
        }
        else {
          this.readFile(files[0])
            .then(file => this.raiseChangeEvent([file]));
        }
      }
      else {
        this.raiseChangeEvent(null);
      }
    }
  }

  private raiseChangeEvent(files: FileContent[] | null): void {
    if (files?.length) {
      const fileNames = files.map(f => f.name);
      this._tooltip = `${fileNames.length} فایل انتخاب شده:\n${fileNames.join('\n')}`;
      this._caption = fileNames.length > 1 ? `${fileNames.length} فایل انتخاب شده` : fileNames[0];
    }
    else {
      this._tooltip = '';
      this._caption = null;
    }

    this.changed.emit(files);

    const value = this.valueFormatter ?
      this.valueFormatter(files) :
      files;
    this.onChange(value);

    this._cdr.detectChanges();
  }

  private readFiles(files: File[]): void {
    Promise.all(files.map(this.readFile))
      .then(files => this.raiseChangeEvent(files));
  }

  @bind()
  private readFile(file: File): Promise<FileContent> {
    return new Promise((resolve, reject) => {
      const lifetime = setTimeout(() => reject('File read timeout'), 2000);
      const reader = new FileReader();
      reader.onload = () => {
        clearTimeout(lifetime);
        const content = reader.result as string;
        const index = content.indexOf(',');
        resolve({
          size: file.size,
          name: file.name,
          type: file.type,
          content: content.substring(index + 1)
        });
      };
      reader.onerror = () => reject();
      reader.readAsDataURL(file);
    });
  }

}

export interface FileContent {
  size: number;
  name: string;
  type: string;
  content: string;
}
