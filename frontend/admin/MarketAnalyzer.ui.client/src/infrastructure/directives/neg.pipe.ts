import { Pipe, PipeTransform } from '@angular/core';
import { formatNegative } from '@utils';

@Pipe({ name: 'neg' })
export class NegPipe implements PipeTransform {

  transform(value: number): string {
    return formatNegative(value);
  }

}
