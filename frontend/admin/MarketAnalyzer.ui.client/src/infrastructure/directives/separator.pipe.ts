import { Pipe, PipeTransform } from '@angular/core';
import { formatCurrency } from '@utils';

@Pipe({ name: 'separator' })
export class SeparatorPipe implements PipeTransform {

  transform(value: string | number | null | undefined): string {
    return (value === null || value === undefined) ?
      '' :
      formatCurrency(Number(value));
  }

}
