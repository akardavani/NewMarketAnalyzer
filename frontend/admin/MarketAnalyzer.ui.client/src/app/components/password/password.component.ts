import { Component, ElementRef, Input, OnDestroy, Renderer2, Signal, computed, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { FormInputBase } from '@directives/form-input-base.directive';
import { InputValidationDirective } from '@directives/input-validation.directive';
import { properties } from '@utils';
import { KeyboardLayoutObject, SimpleKeyboard } from 'simple-keyboard';

@Component({
  selector: 'fd-password',
  imports: [InputValidationDirective, ReactiveFormsModule],
  templateUrl: './password.component.html',
  styleUrl: './password.component.scss',
  providers: [
    {
      multi: true,
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PasswordComponent)
    }
  ]
})
export class PasswordComponent extends FormInputBase implements OnDestroy {

  @Input()
  showStrength = false;

  _showKeyboard = false;
  _passwordType: ('text' | 'password') = 'password';

  _passwordStrength: Signal<number> = computed(() => {
    if (!this.showStrength) {
      return 0;
    }

    const value = this._value;
    if (!value) {
      return 0;
    }

    const total = 8;
    let points = Math.min(2, Math.trunc(value.length / 6));

    if (value.match(/[a-z]/)) {
      points++;
    }

    if (value.match(/[A-Z]/)) {
      points++;
    }

    // If password has at least one number, give 1 point.
    if (value.match(/\d+/)) {
      points += 2;
    }

    // If password has at least one special caracther, give 1 point.
    if (value.match(/.[!,\\, ,|,+,=,`,\,,>,<,/,.,@@,#,$,%,^,&,*,?,_,~,-,(,)]/)) {
      points += 2;
    }

    return (Math.min(total, points) * 100) / total;
  });
  _strengthWidth: Signal<string> = computed(() => `${this._passwordStrength}%`);
  _strengthColor: Signal<string> = computed(() => {
    const strength = this._passwordStrength();
    if (strength === 0) {
      return '';
    }
    else if (strength < 25) {
      return 'veryweak';
    }
    else if (strength < 50) {
      return 'weak';
    }
    else if (strength < 75) {
      return 'medium';
    }
    else {
      return 'strong';
    }
  });
  _strengthLabel: Signal<string> = computed(() => {
    const strength = this._passwordStrength();
    if (strength === 0) {
      return 'غیرقابل قبول';
    }
    else if (strength < 25) {
      return 'خیلی ضعیف';
    }
    else if (strength < 50) {
      return 'ضعیف';
    }
    else if (strength < 75) {
      return 'متوسط';
    }
    else if (strength < 100) {
      return 'قوی';
    }
    else {
      return 'خیلی قوی';
    }
  });

  private _plugin!: SimpleKeyboard;

  constructor(elementRef: ElementRef<HTMLElement>) {
    super(elementRef);
  }

  override ngAfterViewInit(): void {
    super.ngAfterViewInit();

    this._plugin = new SimpleKeyboard({
      rtl: false,
      layoutName: 'normal',
      layout: getKeyboardLayout(),
      display: {
        '{shift}': '⇑',
        '{bksp}': '←',
        '{enter}': 'Enter',
        '{lock}': 'Caps Lock',
        '{tab}': 'Tab',
        '{space}': 'Space',
        '{meta1}': 'ABC',
        '{meta2}': '#+=',
      },
      onChange: (input) => console.log(input),
      onKeyPress: (key) => this.handleShift(key)
    });
  }

  togglePasswordType(): void {
    this._passwordType = this._passwordType == 'password' ? 'text' : 'password';
  }

  toogleKeyboard(): void {
    this._showKeyboard = !this._showKeyboard;

    if (this._showKeyboard) {
      this._plugin.setOptions({
        layout: getKeyboardLayout()
      });
    }
  }

  ngOnDestroy(): void {
    this._plugin.destroy();
  }

  private handleShift(key: string): void {
    /**
     * If you want to handle the shift and caps lock buttons
     */
    if (key === "{shift}") {
      const currentLayout = this._plugin.options.layoutName;
      const shiftToggle = currentLayout === "normal" ? "shift" : "normal";
      this._plugin.setOptions({
        layoutName: shiftToggle
      });
    }
  }

}

function getKeyboardLayout(): KeyboardLayoutObject {
  const layout = {
    normal: ["q w e r t y u i o p", "a s d f g h j k l {bksp}", "z x c v b n m , . {shift}", "{space} {meta1}"],
    shift: ["Q W E R T Y U I O P", "A S D F G H J K L {bksp}", "Z X C V B N M ! ? {shift}", "{space} {meta1}"],
    meta1: ["1 2 3 4 5 6 7 8 9 0", "- / : ; ( ) € & @ {bksp}", ". , ? ! ' \" {meta2}", "{space} {normal}"],
    meta2: ["[ ] { } # % ^ * + =", "_ \\ | ~ < > $ £ ¥ {bksp}", ". , ? ! ' \" {meta1}", "{space} {normal}"],

    //'default': [
    //  '` 1 2 3 4 5 6 7 8 9 0 - = {bksp}',
    //  '{tab} q w e r t y u i o p [ ] \\',
    //  '{lock} a s d f g h j k l ; \' {enter}',
    //  '{shift} z x c v b n m , . / {shift}',
    //  '.com @ {space}'
    //],
    //'shift1': [
    //  '~ ! @ # $ % ^ &amp; * ( ) _ + {bksp}',
    //  '{tab} Q W E R T Y U I O P { } |',
    //  '{lock} A S D F G H J K L : " {enter}',
    //  '{shift} Z X C V B N M &lt; &gt; ? {shift}',
    //  '.com @ {space}'
    //]
  };

  properties(layout)
    .forEach(key => {
      const len = layout[key].length;
      for (let i = 0; i < len; i++) {
        layout[key][i] = layout[key][i]
          .split(' ')
          .sort((left, right) => {
            return 0;
            //if (i == 0 && left == '{bksp}') {
            //  return 1;
            //}
            //else if (i == 1 && left == '{tab}') {
            //  return -10;
            //}
            //else {
            //  return 0.5 - Math.random();
            //}
          })
          .join(' ');
      }
    });

  return layout;
}
