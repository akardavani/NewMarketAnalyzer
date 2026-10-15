import { Pipe, PipeTransform } from '@angular/core';
import { formatCurrency } from '@utils';

@Pipe({
  name: 'currency'
})
export class CurrencyPipe implements PipeTransform {

  transform(value: string | number | null | undefined ,fix:number=5): string {
    return (value === null || value === undefined) ?
      '' :
       value != null
      ? parseFloat((+value).toFixed(fix))
        .toString()
        .replace(/\B(?=(\d{3})+(?!\d))/g, ',')
      : '';
  }

}
