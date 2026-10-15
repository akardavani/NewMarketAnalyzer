import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { FileContent } from '@components/file-input/file-input.component';
import { DialogFormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { Nullable } from '@typings';
import { apiUrl } from '@utils';

@Component({
  selector: 'fd-create-prospectus',
  imports: [...GlobalImports],
  templateUrl: './create-prospectus.component.html',
  styleUrl: './create-prospectus.component.scss',
})
export class CreateProspectusComponent extends DialogFormBase<CreateProspectus> {
  returnFileContent = (files: FileContent[] | null) => {
    if (!files) return null;
    return files.length ? files[0] : null;
  };

  protected override submitCore(): void {
    this.postData(apiUrl('Prospectus/Create')).subscribe((response) => {
      if (response.successed) {
        this.growl.success({
          content: 'عملیات بارگذاری فایل با موفقیت انجام شد.',
        });
        this.close();
      }
    });
  }
  protected override buildForm(
    builder: FormBuilder,
  ): FormGroupOf<CreateProspectus> {
    return builder.group({
      date: formControl<Nullable<Date>>('شناسه نقش', null, (context) =>
        context.required(),
      ),
      description: formControl<Nullable<string>>('نقش', null, (context) =>
        context.required(),
      ),
      fileContent: formControl<Nullable<string>>(
        'فایل رزومه',
        null,
        (context) => context.required(),
      ),
    });
  }
}
export interface CreateProspectus {
  date: Date;
  description: string;
  fileContent: string;
}
