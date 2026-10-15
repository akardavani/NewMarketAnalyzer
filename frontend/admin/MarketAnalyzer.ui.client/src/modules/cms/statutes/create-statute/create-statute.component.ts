import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { FileContent } from '@components/file-input/file-input.component';
import { DialogFormBase } from '@directives/form-base';
import { formControl, FormGroupOf } from '@form-utils';
import { GlobalImports } from '@imports';
import { Nullable } from '@typings';
import { apiUrl } from '@utils';

@Component({
  selector: 'fd-create-statute',
  imports: [...GlobalImports],
  templateUrl: './create-statute.component.html',
  styleUrl: './create-statute.component.scss',
})
export class CreateStatuteComponent extends DialogFormBase<CreateStatute> {
  returnFileContent = (files: FileContent[] | null) => {
    if (!files) return null;
    return files.length ? files[0] : null;
  };

  protected override submitCore(): void {
    this.postData(apiUrl('Statute/Create')).subscribe((response) => {
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
  ): FormGroupOf<CreateStatute> {
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
export interface CreateStatute {
  date: Date;
  description: string;
  fileContent: string;
}
