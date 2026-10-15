import { InjectionToken } from "@angular/core";
import { FormArray, FormControl, FormGroup, ɵElement } from "@angular/forms"
import { Validation, ValidationContext } from "@validations"

//export const FORM_DATA = new InjectionToken<any>("FORM_DATA");

type FormControlsOf<T> = {
  [K in Extract<keyof T, string>]:
  T[K] extends (infer U)[] ? U extends object ? FormArray<FormGroupOf<U>> : FormControl<T[K] | null> : ɵElement<T[K], null>;
};

export type FormGroupOf<T> = FormGroup<FormControlsOf<T>>;


export function formControl<TValue, TModel = any>(propertyName: string, value: TValue, validationFactory?: ValidationFactory<TModel>):
  FormControl<TValue | null> {
  const context = Validation.enrich<TModel>({ propertyName: propertyName });
  return new FormControl(value, (validationFactory ? validationFactory(context) : context).build())
}

export function formArray<TValue, TModel = any>(propertyName: string, controls: FormGroupOf<TValue>[], validationFactory?: ValidationFactory<TModel>):
  FormArray<FormGroupOf<TValue>> {
  const context = Validation.enrich<TModel>({ propertyName: propertyName });
  return new FormArray(controls, (validationFactory ? validationFactory(context) : context).build())
}

export interface ValidationFactory<TModel = any> {
  (context: ValidationContext<TModel>): ValidationContext<TModel>;
}

export interface DataNormalizer<TData = any, TResult = any> {
  (data: TData): TResult;
}

export const REFRESHABLE_HOST = new InjectionToken<IRefreshableHost>("REFRESHABLE_HOST");
export const VALUE_ACCESSOR_OBSERVER = new InjectionToken<readonly ControlValueAccessorObserver[]>("VALUE_ACCESSOR_OBSERVER");

export interface ControlValueAccessorObserver {
  valueChanged(value: any): void;
}

export interface IRefreshableHost {
  refresh(): void;
}
