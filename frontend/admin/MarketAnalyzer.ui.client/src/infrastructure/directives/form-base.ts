import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Directive, inject, OnDestroy } from '@angular/core';
import { FormArray, FormBuilder, FormControl } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import {
  DataNormalizer,
  FormGroupOf,
  IRefreshableHost,
  REFRESHABLE_HOST,
} from '@form-utils';
import { GrowlService } from '@services/growl.service';
import { ApiResult, Nullable } from '@typings';
import { buildUrl, deepClone, properties } from '@utils';
import {
  catchError,
  finalize,
  Observable,
  Subject,
  takeUntil,
  throwError,
} from 'rxjs';

@Directive()
export abstract class FormBase<T> implements OnDestroy {
  loading = false;
  readonly formGroup: FormGroupOf<T>;

  protected _host?: IRefreshableHost;
  protected readonly growl: GrowlService;
  protected cancellation?: Subject<void>;
  protected readonly _httpClient: HttpClient;
  protected readonly _cdr: ChangeDetectorRef;
  protected _initValue: T;

  constructor() {
    this.growl = inject(GrowlService);
    this._httpClient = inject(HttpClient);
    this._cdr = inject(ChangeDetectorRef);
    this.formGroup = this.buildForm(inject(FormBuilder));
    this._host = inject<any>(REFRESHABLE_HOST, { optional: true });
    this._initValue = deepClone(this.formValue);
  }

  protected get formValue(): T {
    return this.formGroup.getRawValue() as T;
  }

  submit(): void {
    if (this.formGroup.valid) {
      this.submitCore();
    }
  }

  ngOnDestroy(): void {
    if (this.cancellation) {
      this.cancellation.next();
      this.cancellation.complete();
      this.cancellation = undefined;
    }
  }

  reset(): void {
    this.formGroup.reset(deepClone(this._initValue) as any);
  }

  protected getData<TResponse, TData = any>(
    url: string,
    data?: TData,
  ): Observable<ApiResult<TResponse>> {
    if (!this.cancellation) {
      this.cancellation = new Subject<void>();
    }

    return this._httpClient
      .get<ApiResult<TResponse>>(buildUrl(url, data))
      .pipe(takeUntil(this.cancellation));
  }

  protected refreshHost(): void {
    this?._host?.refresh();
  }

  protected postData<TResponse = void, TResult = T>(
    url: string,
    normalizer?: DataNormalizer<T, TResult>,
  ): Observable<ApiResult<TResponse>> {
    this.setLoader(true);
    if (!this.cancellation) {
      this.cancellation = new Subject<void>();
    }

    const requestData = normalizer
      ? normalizer(this.formValue)
      : this.formValue;

    return this._httpClient.post<ApiResult<TResponse>>(url, requestData).pipe(
      takeUntil(this.cancellation),
      //tap(() => this.setLoader(false)),
      catchError((error) => {
        this.setLoader(false);
        return throwError(() => error);
      }),
      finalize(() => this.setLoader(false)),
    );
  }

  protected fillForm(formData: T): void {
    properties(formData).forEach((property) => {
      const control = this.formGroup.get(property);
      if (control) {
        const value = formData[property];
        if (Array.isArray(value)) {
          const values: any = value.map((e) => new FormControl(e));
          control.setValue(values);
        } else {
          control.setValue(value as any);
        }
      }
    });
  }

  protected subscribeChanges<K extends keyof T>(
    property: K,
    callback: (value: T[K]) => void,
  ): void {
    this.formGroup.controls[property].valueChanges.subscribe(callback);
  }

  protected setControlValue<K extends keyof T>(
    controlName: K,
    value: Nullable<T[K]>,
  ): void {
    const control = this.formGroup.get(controlName as string);
    if (!control) {
      throw new Error(`The control [${String(controlName)}] does not exist.`);
    }

    if (control instanceof FormArray) {
      throw new Error('FormArray not supported');
    } else {
      control.setValue(value as any);
    }
  }

  protected setLoader(loading: boolean): void {
    this.loading = loading;
    this._cdr.markForCheck();
  }

  protected abstract submitCore(): void;
  protected abstract buildForm(builder: FormBuilder): FormGroupOf<T>;
}

export abstract class DialogFormBase<T> extends FormBase<T> {
  protected readonly _dialog: MatDialogRef<DialogFormBase<T>>;

  constructor() {
    super();
    this._dialog = inject(MatDialogRef<DialogFormBase<T>>);
  }

  close(result?: any): void {
    this._dialog.close(result);
    this.refreshHost();
  }
}
