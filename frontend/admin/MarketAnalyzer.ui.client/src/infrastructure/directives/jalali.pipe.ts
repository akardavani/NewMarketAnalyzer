import { Pipe, PipeTransform } from '@angular/core';
import { formatDateTime, parseDateTime } from '@utils';
import { DateTime } from '@xobin/globalization';

@Pipe({ name: 'jalali' })
export class JalaliPipe implements PipeTransform {

  transform(value: Date | DateTime | string | null | undefined, format: string = 'yyyy/MM/dd'): string {
    if (value instanceof DateTime) {
      return formatDateTime(value, format);
    }

    const date = parseDateTime(value);
    return date ? this.transform(date, format) : '';
  }

}
