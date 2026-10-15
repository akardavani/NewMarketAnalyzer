import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { formatString } from './utils';

export interface ValidationContextOptions {
  propertyName: string;
}

export interface ValidationContext<TModel = any> {
  build(): Array<ValidatorFn>;
  min(min: number): ValidationContext<TModel>;
  max(max: number): ValidationContext<TModel>;
  required(): ValidationContext<TModel>;
  requiredTrue(): ValidationContext<TModel>;
  email(): ValidationContext<TModel>;
  len(minLength: number, maxLength: number): ValidationContext<TModel>;
  minLength(minLength: number): ValidationContext<TModel>;
  maxLength(maxLength: number): ValidationContext<TModel>;
  pattern(pattern: string | RegExp): ValidationContext<TModel>;
  compare(otherPropertyName: string): ValidationContext<TModel>;
  requiredIf(predicate: (model: TModel) => boolean): ValidationContext<TModel>;
  add(factory: (context: ValidationContext<TModel>) => ValidatorFn): ValidationContext<TModel>;
}

export class Validation {

  static enrich<TModel = any>(options: ValidationContextOptions): ValidationContext<TModel> {
    return new _ValidationContext(options);
  }

}

//export interface Validator {
//  (context: ValidationContext): ValidatorFn;
//}

class _ValidationContext<TModel = any> implements ValidationContext<TModel> {

  readonly propertyName!: string;

  private readonly _validators: ValidatorFn[] = [];

  constructor(readonly options: ValidationContextOptions) {
    Object.assign(this, options);
  }

  build(): Array<ValidatorFn> {
    return this._validators;
  }

  min(min: number): ValidationContext<TModel> {
    const fn = Validators.min(min);
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (fn(control) == null) ?
        null :
        { 'min': formatString('مقدار {0} نباید کمتر از {1} باشد.', this.propertyName, min) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  max(max: number): ValidationContext<TModel> {
    const fn = Validators.max(max);
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (fn(control) == null) ?
        null :
        { 'max': formatString('مقدار {0} نباید بیشتر از {1} باشد.', this.propertyName, max) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  required(): ValidationContext<TModel> {
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (Validators.required(control) == null) ?
        null :
        { 'required': formatString('{0} را وارد کنید.', this.propertyName) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  len(minLength: number, maxLength: number): ValidationContext<TModel> {
    const minFn = Validators.minLength(minLength);
    const maxFn = Validators.maxLength(maxLength);
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (minFn(control) == null && maxFn(control) == null) ?
        null :
        { 'len': formatString('طول {0} باید بین {1} و {1} باشد.', this.propertyName, minLength, maxLength) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  requiredTrue(): ValidationContext<TModel> {
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (Validators.requiredTrue(control) == null) ?
        null :
        { 'required': formatString('شما باید {0} را بپذیرید.', this.propertyName) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  email(): ValidationContext<TModel> {
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (Validators.email(control) == null) ?
        null :
        { 'email': formatString('مقدار {0} آدرس معتبری نمیباشد.', this.propertyName) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  minLength(minLength: number): ValidationContext<TModel> {
    const fn = Validators.minLength(minLength);
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (fn(control) == null) ?
        null :
        { 'minLength': formatString('حداقل طول {0} باید {1} باشد.', this.propertyName, minLength) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  maxLength(maxLength: number): ValidationContext<TModel> {
    const fn = Validators.maxLength(maxLength);
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (fn(control) == null) ?
        null :
        { 'maxLength': formatString('حداکثر طول {0} باید {1} باشد.', this.propertyName, maxLength) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  pattern(pattern: string | RegExp): ValidationContext<TModel> {
    const fn = Validators.pattern(pattern);
    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (fn(control) == null) ?
        null :
        { 'pattern': formatString("مقدار {0} طبق قاعده نیست.", this.propertyName, pattern) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  compare(otherPropertyName: string): ValidationContext<TModel> {
    //Could not find a property named {0}.

    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      return (control == null) ?
        null :
        { 'compare': formatString("مقدار {0} با مقدار {1} برابر نیست.", this.propertyName) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  requiredIf(predicate: (model: TModel) => boolean): ValidationContext<TModel> {

    const validatorFn = (control: AbstractControl): ValidationErrors | null => {
      const model = control.parent as FormGroup;
      return (!model || !predicate(model.getRawValue()) || Validators.required(control) == null) ?
        null :
        { 'required': formatString('{0} را وارد کنید.', this.propertyName) };
    };

    this._validators.push(validatorFn);
    return this;
  }

  add(factory: (context: ValidationContext<TModel>) => ValidatorFn): ValidationContext<TModel> {
    const fn = factory(this);
    this._validators.push(fn);
    return this;
  }

}


//The {0} field is not a valid credit card number.
//CustomValidator_ValidationError: The {0} field is not valid.
//The {0} field must be equal to {1}.
//The {0} field only accepts files with the following extensions: {1}.
//The {0} field must be greater than or equal to {1}.
//The {0} field must be a collection with a minimum length of {2} and a maximum length of {1}.
//The {0} field must be less than or equal to {1}.
//
//MaxLengthValidator must have a Length value that is greater than zero.
//The {0} field must be a string or array type with a maximum length of '{1}'.
//MinLengthValidator must have a Length value that is zero or greater.
//The {0} field must be a string or array type with a minimum length of '{1}'.
//The {0} field must not be equal to {1}.
//The {0} field value cannot be null.
//The minimum and maximum values must be set.
//The {0} field must be between {1} and {2}.
//The maximum value '{0}' must be greater than or equal to the minimum value '{1}'.
//
//The {0} field must be a string with a minimum length of {2} and a maximum length of {1}.
//The {0} field is not a valid fully-qualified http, https, or ftp URL.
//
