import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { FileContent } from '@components/file-input/file-input.component';
import { DialogFormBase } from '@directives/form-base';
import { FormGroupOf, formControl } from '@form-utils';
import { GlobalImports } from '@imports';
import { Nullable } from '@typings';
import { apiUrl } from '@utils';

@Component({
  selector: 'fd-create-investment-policy-statement',
  imports: [...GlobalImports],
  templateUrl: './create-investment-policy-statement.component.html',
  styleUrl: './create-investment-policy-statement.component.scss',
})
export class CreateInvestmentPolicyStatementComponent extends DialogFormBase<CreateInvestmentPolicyStatement> {
  returnFileContent = (files: FileContent[] | null) => {
    if (!files) return null;
    return files.length ? files[0] : null;
  };

  protected override submitCore(): void {
    this.postData(apiUrl('InvestmentPolicyStatement/Create')).subscribe(
      (response) => {
        if (response.successed) {
          this.growl.success({
            content: 'عملیات بارگذاری فایل با موفقیت انجام شد.',
          });
          this.close();
        }
      },
    );
  }
  protected override buildForm(
    builder: FormBuilder,
  ): FormGroupOf<CreateInvestmentPolicyStatement> {
    return builder.group({
      date: formControl<Nullable<Date>>('شناسه نقش', null, (context) =>
        context.required(),
      ),
      description: formControl<Nullable<string>>('نقش', null, (context) =>
        context.required(),
      ),
      isActive: formControl<Nullable<boolean>>('فعال', false, (context) =>
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
export interface CreateInvestmentPolicyStatement {
  date: Date;
  description: string;
  isActive: boolean;
  fileContent: string;
}
