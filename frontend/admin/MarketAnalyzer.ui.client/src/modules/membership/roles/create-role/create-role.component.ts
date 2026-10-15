import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { DialogFormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { Nullable } from '@typings';
import { apiUrl } from '@utils';

@Component({
  selector: 'fd-create-role',
  imports: [...GlobalImports],
  templateUrl: './create-role.component.html',
  styleUrl: './create-role.component.scss'
})
export class CreateRoleComponent extends DialogFormBase<CreateRole> {

  constructor() {
    super();
  }

  protected override submitCore(): void {
    this.postData(apiUrl('Role/Create'))
      .subscribe(response => {
        if (response.successed) {
          this.growl.success({ content: 'ایجاد نقش جدید با موفقیت انجام شد.' });
          this.close();
        }
      });
  }

  protected override buildForm(builder: FormBuilder): FormGroupOf<CreateRole> {
    return builder.group({
      name: formControl<Nullable<string>>('نام', null, context => context.required()),
      displayName: formControl<Nullable<string>>('نام نمایشی', null, context => context.required()),
      isActive: formControl<Nullable<boolean>>('فعال', true, context => context.required()),
    });
  }

}


interface CreateRole {
  name: string;
  displayName: string;
  isActive: boolean;
}
