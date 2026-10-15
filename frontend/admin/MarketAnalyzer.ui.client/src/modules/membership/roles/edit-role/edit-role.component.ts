import { Component, inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { DialogFormBase, FormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { Nullable } from '@typings';
import { Roles } from '../../membership-models';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { apiUrl } from '@utils';
import { GlobalImports } from '@imports';

@Component({
  selector: 'fd-edit-role',
  imports: [...GlobalImports],
  templateUrl: './edit-role.component.html',
  styleUrl: './edit-role.component.scss'
})
export class EditRoleComponent extends DialogFormBase<EditRole> {

  entity!: Roles;

  constructor() {
    super();

    if (!this.entity) {
      this.growl.error({ content: 'کاربر پیدا نشد.' });
      this.close();
    }

    this.setLoader(true);

    this.getData<EditRole>(apiUrl('Role/Get'), { id: this.entity.id })
      .subscribe(response => {
        if (response.successed) {
          this.formGroup.patchValue({
            name: response.data.name,
            displayName: response.data.displayName,
            isActive: response.data.isActive
          });
        }
        else {
          this.close();
        }
        this.setLoader(false);
      });
  }

  protected override submitCore(): void {
    this.postData(apiUrl('Role/Update'))
      .subscribe(response => {
        if (response.successed) {
          this.growl.success({ content: 'عملیات ویرایش نقش با موفقیت انجام شد.' });
          this.close();
        }
      });
  }

  protected override buildForm(builder: FormBuilder): FormGroupOf<EditRole> {
    this.entity = inject(MAT_DIALOG_DATA);

    return builder.group({
      id: formControl<Nullable<number>>('شناسه', this.entity.id, context => context.required()),
      name: formControl<Nullable<string>>('نام', null, context => context.required()),
      displayName: formControl<Nullable<string>>('نام نمایشی', null, context => context.required()),
      isActive: formControl<Nullable<boolean>>('فعال', true, context => context.required())
    });
  }

}

interface EditRole {
  id: number;
  name: string;
  displayName: string;
  isActive: boolean;
}
