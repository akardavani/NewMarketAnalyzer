import { RouterLink } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AsyncPipe, CommonModule } from '@angular/common';
import { PageWrapperComponent } from '@components/page-wrapper/page-wrapper.component';
import { FloatLabelComponent } from '@components/float-label/float-label.component';
import { NegativeDirective } from '@directives/negative.directive';
import { SeparatorPipe } from '@directives/separator.pipe';
import { InputValidationDirective } from '@directives/input-validation.directive';
import { ScrollbarComponent } from '@components/scrollbar/scrollbar.component';
import { NumbersOnlyDirective } from '@directives/numbers-only.directive';
import { AutoNumericDirective } from '@directives/auto-numeric.directive';
import { DateInputComponent } from '@components/date-input/date-input.component';
import { JalaliPipe } from '@directives/jalali.pipe';
import { AgGridAngular } from 'ag-grid-angular';
import { LoaderComponent } from '@components/loader/loader.component';
import { SelectListComponent } from '@components/select-list/select-list.component';
import { AutocompleteComponent } from '@components/autocomplete/autocomplete.component';
import { DropDownComponent } from '@components/drop-down/drop-down.component';
import { MatDialogModule } from '@angular/material/dialog';
import { ErrorMessageDirective } from '@directives/error-for.directive';
import { NegPipe } from '@directives/neg.pipe';
import {
  InputControlAccessorDirective,
  InputValueAccessorDirective,
} from '@directives/input-controll-accessor';
import { OptionComponent } from '@components/drop-down/option.component';
import { FileInputComponent } from '@components/file-input/file-input.component';
import { FieldsetComponent } from '@components/fieldset/fieldset.component';
import { CurrencyPipe } from '@directives/currency.pipe';
import { PasswordComponent } from '@components/password/password.component';
import { CheckboxComponent } from '@components/checkbox/checkbox.component';
import { CheckboxGroupComponent } from '@components/checkbox-group/checkbox-group.component';
import { CaptchaComponent } from '@components/captcha/captcha.component';
import { DialogAutoHeightDirective } from '@directives/dialog-auto-height.directive';

export const GlobalImports = [
  RouterLink,
  FormsModule,
  ReactiveFormsModule,
  CommonModule,

  //Angular
  AsyncPipe,
  MatDialogModule,

  //Pipes
  NegPipe,
  JalaliPipe,
  CurrencyPipe,
  SeparatorPipe,

  //Directivesf
  NegativeDirective,
  AutoNumericDirective,
  NumbersOnlyDirective,
  ErrorMessageDirective,
  InputValidationDirective,
  DialogAutoHeightDirective,
  InputValueAccessorDirective,
  InputControlAccessorDirective,

  //Componens
  OptionComponent,
  LoaderComponent,
  CaptchaComponent,
  DropDownComponent,
  FieldsetComponent,
  CheckboxComponent,
  PasswordComponent,
  ScrollbarComponent,
  FileInputComponent,
  DateInputComponent,
  FloatLabelComponent,
  SelectListComponent,
  PageWrapperComponent,
  AutocompleteComponent,
  CheckboxGroupComponent,
];

export const GridImports = [...GlobalImports, AgGridAngular];
