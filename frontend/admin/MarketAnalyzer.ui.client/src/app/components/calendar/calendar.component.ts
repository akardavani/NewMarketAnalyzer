import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { MatCalendar, MatDatepickerModule } from '@angular/material/datepicker';
import { CalendarHeaderComponent } from '@components/calendar/calendar-header/calendar-header.component';
import {
  DateOnly,
  provideXobinDateAdapter,
} from '@components/calendar/xobin-date-adapter';
import { getValue, setValue } from '@utils';
import { CultureInfo, DateTime, DayOfWeek } from '@xobin/globalization';

@Component({
  selector: 'fd-calendar',
  imports: [MatDatepickerModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss',
  encapsulation: ViewEncapsulation.None,
  providers: [provideXobinDateAdapter()],
  host: {
    class: 'fd-calendar',
  },
})
export class CalendarComponent implements AfterViewInit {
  header = CalendarHeaderComponent;

  @ViewChild('calendar')
  calendar!: MatCalendar<DateOnly>;
  @Input()
  minDate: DateOnly | null = null;

  @Input()
  maxDate: DateOnly | null = null;
  readonly selectedChange = new EventEmitter<Date | null>();

  constructor(private readonly _cdr: ChangeDetectorRef) {}

  @Input()
  get value(): DateOnly | null {
    return getValue(this, 'value');
  }
  set value(value: DateOnly | null) {
    setValue(this, 'value', value);
    if (this.calendar) {
      this.calendar.activeDate = value || this.today;
    }
    this._cdr.markForCheck();
  }

  get today(): DateOnly {
    return new DateOnly(DateTime.today, CultureInfo.currentCulture.calendar);
  }

  ngAfterViewInit(): void {
    this.calendar.activeDate = this.value || this.today;
  }

  goToday(): void {
    this.calendar.activeDate = this.today;
    this.calendar.currentView = 'month';
    this._cdr.markForCheck();
  }

  onSelect(value: DateOnly | null): void {
    setValue(this, 'value', value);
    this.selectedChange.emit(value?.toDate() ?? null);
    this._cdr.markForCheck();
  }

  dateClassSelector(
    date: DateOnly,
    view: 'month' | 'year' | 'multi-year',
  ): string | string[] {
    if (view === 'month') {
      if (date.dayOfWeek === DayOfWeek.Friday) {
        return 'friday-day';
      }
    }

    return '';
  }
}
