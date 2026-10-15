import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[numbersOnly]',
  standalone: true
})
export class NumbersOnlyDirective {

  @Input()
  allowDecimals: boolean = true;

  @Input()
  allowSign: boolean = false;

  @Input()
  decimalSeparator: string = '.';

  previousValue: string = '';

  //  Regular expressions
  private readonly _integerUnsigned: string = '^[0-9]*$';
  private readonly _integerSigned: string = '^-?[0-9]+$';
  private readonly _decimalUnsigned: string = '^[0-9]+(.[0-9]+)?$';
  private readonly _decimalSigned: string = '^-?[0-9]+(.[0-9]+)?$';

  constructor(private readonly _hostElement: ElementRef<HTMLInputElement>) {

  }

  @HostListener('change', ['$event'])
  onChange(event: Event) {
    this.validateValue((event.target as HTMLInputElement).value);
  }

  @HostListener('paste', ['$event'])
  onPaste(event: ClipboardEvent) {
    // get and validate data from clipboard
    const value = event.clipboardData!.getData('text/plain');
    this.validateValue(value);
    event.preventDefault();
  }

  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    const cursorPosition: number = (e.target as any)['selectionStart'];
    const originalValue: string = (e.target as any)['value'];
    const key = e.key;
    const controlOrCommand = (e.ctrlKey === true || e.metaKey === true);
    const signExists = originalValue.includes('-');
    const separatorExists = originalValue.includes(this.decimalSeparator);

    // allowed keys apart from numeric characters
    const allowedKeys = [
      'Backspace', 'ArrowLeft', 'ArrowRight', 'Escape', 'Tab'
    ];

    // when decimals are allowed, add
    // decimal separator to allowed codes when
    // its position is not close to the the sign (-. and .-)
    const separatorIsCloseToSign = (signExists && cursorPosition <= 1);
    if (this.allowDecimals && !separatorIsCloseToSign && !separatorExists) {
      if (this.decimalSeparator == '.')
        allowedKeys.push('.');
      else
        allowedKeys.push(',');
    }

    // when minus sign is allowed, add its
    // key to allowed key only when the
    // cursor is in the first position, and
    // first character is different from
    // decimal separator
    const firstCharacterIsSeparator = (originalValue.charAt(0) != this.decimalSeparator);
    if (this.allowSign && !signExists &&
      firstCharacterIsSeparator && cursorPosition == 0) {
      allowedKeys.push('-');
    }

    // allow some non-numeric characters
    if (allowedKeys.indexOf(key) != -1 ||
      // Allow: Ctrl+A and Command+A
      (key == 'a' && controlOrCommand) ||
      // Allow: Ctrl+C and Command+C
      (key == 'c' && controlOrCommand) ||
      // Allow: Ctrl+V and Command+V
      (key == 'v' && controlOrCommand) ||
      // Allow: Ctrl+X and Command+X
      (key == 'x' && controlOrCommand)) {
      // let it happen, don't do anything
      return;
    }

    // save value before keydown event
    this.previousValue = originalValue;

    // allow number characters only
    if ((new RegExp(this._integerUnsigned)).test(key))
      return;
    else
      e.preventDefault();
  }

  private validateValue(value: string): void {
    if (value == null || value === '') {
      return;
    }

    // choose the appropiate regular expression
    let regex: string;
    if (!this.allowDecimals && !this.allowSign) regex = this._integerUnsigned;
    if (!this.allowDecimals && this.allowSign) regex = this._integerSigned;
    if (this.allowDecimals && !this.allowSign) regex = this._decimalUnsigned;
    if (this.allowDecimals && this.allowSign) regex = this._decimalSigned;

    // when a numbers begins with a decimal separator,
    // fix it adding a zero in the beginning
    const firstCharacter = value.charAt(0);
    if (firstCharacter == this.decimalSeparator)
      value = 0 + value;

    // when a numbers ends with a decimal separator,
    // fix it adding a zero in the end
    const lastCharacter = value.charAt(value.length - 1);
    if (lastCharacter == this.decimalSeparator)
      value = value + 0;

    // test number with regular expression, when
    // number is invalid, replace it with a zero
    const valid: boolean = (new RegExp(regex!)).test(value);
    this._hostElement.nativeElement.value = valid ? value : '0';
  }

}
