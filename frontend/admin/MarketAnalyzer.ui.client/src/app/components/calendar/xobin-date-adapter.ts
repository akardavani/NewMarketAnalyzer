import { Injectable, Provider } from '@angular/core';
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MatDateFormats,
} from '@angular/material/core';
import {
  Calendar,
  CultureInfo,
  DateTime,
  DayOfWeek,
} from '@xobin/globalization';
import { formatDateTime } from '@utils';

@Injectable()
export class XobinDateAdapter extends DateAdapter<DateOnly> {
  calendar: Calendar;

  constructor() {
    super();
    this.calendar = CultureInfo.currentCulture.calendar;
  }

  override getYear(date: DateOnly): number {
    return date.year;
  }

  override getMonth(date: DateOnly): number {
    return date.month;
  }

  override getDate(date: DateOnly): number {
    return date.day;
  }

  override getDayOfWeek(date: DateOnly): number {
    return date.dayOfWeek;
  }

  override getMonthNames(style: 'long' | 'short' | 'narrow'): string[] {
    switch (style) {
      case 'long':
        return [...CultureInfo.currentCulture.dateTimeFormat.monthNames];
      case 'short':
        return [
          ...CultureInfo.currentCulture.dateTimeFormat.abbreviatedMonthNames.filter(
            (e) => !!e,
          ),
        ];
      case 'narrow':
        return CultureInfo.currentCulture.dateTimeFormat.abbreviatedMonthNames
          .filter((e) => !!e)
          .map((_, i) => String(i + 1));
      default:
        throw new Error(`Invalid style: ${style}`);
    }
  }

  override getDateNames(): string[] {
    return [...Array(31).keys()].map((i) => String(i + 1));
  }

  override getDayOfWeekNames(style: 'long' | 'short' | 'narrow'): string[] {
    switch (style) {
      case 'long':
        return [...CultureInfo.currentCulture.dateTimeFormat.dayNames];
      case 'short':
        return [...CultureInfo.currentCulture.dateTimeFormat.shortestDayNames];
      case 'narrow':
        return [...CultureInfo.currentCulture.dateTimeFormat.shortestDayNames];
      default:
        throw new Error(`Invalid style: ${style}`);
    }
  }

  override getYearName(date: DateOnly): string {
    return String(date.year);
  }

  override getFirstDayOfWeek(): number {
    return CultureInfo.currentCulture.dateTimeFormat.firstDayOfWeek;
  }

  override getNumDaysInMonth(date: DateOnly): number {
    return date.getDaysInMonth();
  }

  override clone(date: DateOnly): DateOnly {
    return date.clone();
  }

  override createDate(year: number, month: number, date: number): DateOnly {
    if (month < 0 || month > 11) {
      throw Error(
        `Invalid month index "${month}". Month index has to be between 0 and 11.`,
      );
    }

    if (date < 1) {
      throw Error(`Invalid date "${date}". Date has to be greater than 0.`);
    }

    // Passing the year to the constructor causes year numbers <100 to be converted to 19xx.
    // To work around this we use `setFullYear` and `setHours` instead.
    const dateTime = new DateTime(year, month + 1, date, this.calendar);
    return new DateOnly(dateTime, this.calendar);
  }

  override today(): DateOnly {
    return new DateOnly(DateTime.today, this.calendar);
  }

  override parse(value: any, parseFormat: any): DateOnly | null {
    // We have no way using the native JS Date to set the parse format or locale, so we ignore these
    // parameters.
    //if (typeof value == 'number') {
    //  return DateOnly.fromDateTime(new DateTime(value), this.calendar);
    //}
    throw new Error('Not supported');
    //return value ? new DateTime(Date.parse(value)) : null;
  }

  override format(date: DateOnly, displayFormat: any): string {
    return date.toString(displayFormat);
  }

  override addCalendarYears(date: DateOnly, years: number): DateOnly {
    return this.addCalendarMonths(date, years * 12);
  }
  override addCalendarMonths(date: DateOnly, months: number): DateOnly {
    const total = this.getYear(date) * 12 + (this.getMonth(date) - 1) + months;
    const year = Math.floor(total / 12);
    const month = total - year * 12 + 1;
    const firstOfMonth = this.createDate(year, month, 1);

    const day = Math.min(
      this.getDate(date),
      this.getNumDaysInMonth(firstOfMonth),
    );
    return this.createDate(year, month, day);
  }

  override addCalendarDays(date: DateOnly, days: number): DateOnly {
    return date.addDays(days);
  }

  override toIso8601(date: DateOnly): string {
    return date.toString('o');
  }

  override isDateInstance(obj: any): boolean {
    return obj instanceof DateOnly;
  }

  override isValid(date: DateOnly): boolean {
    return date == null || true;
  }

  override invalid(): DateOnly {
    return new DateOnly(this.calendar.minSupportedDateTime, this.calendar);
  }
}

export class DateOnly {
  readonly year: number;
  readonly month: number;
  readonly day: number;

  readonly date: DateTime;

  constructor(
    date: DateTime,
    readonly calendar: Calendar,
  ) {
    this.date = date.date;
    this.year = calendar.getYear(this.date);
    this.month = calendar.getMonth(this.date) - 1;
    this.day = calendar.getDayOfMonth(this.date);
  }

  get ticks(): number {
    return this.date.ticks;
  }

  get dayOfWeek(): DayOfWeek {
    return this.calendar.getDayOfWeek(this.date);
  }

  addYears(years: number): DateOnly {
    const date = this.calendar.addYears(this.date, years);
    return new DateOnly(date, this.calendar);
  }

  addMonths(months: number): DateOnly {
    const date = this.calendar.addMonths(this.date.date, months);
    return new DateOnly(date, this.calendar);
  }

  addDays(days: number): DateOnly {
    const date = this.calendar.addDays(this.date, days);
    return new DateOnly(date, this.calendar);
  }

  getDaysInMonth(): number {
    return this.calendar.getDaysInMonth(this.year, this.month + 1);
  }

  clone(): DateOnly {
    return new DateOnly(this.date, this.calendar);
  }

  toString(format: string): string {
    return formatDateTime(this.date, format);
  }

  toDate(): Date {
    return new Date(this.date.year, this.date.month - 1, this.date.day);
  }

  static fromDate(date: Date): DateOnly {
    const dt = new DateTime(
      date.getFullYear(),
      date.getMonth() + 1,
      date.getDate(),
      date.getHours(),
      date.getMinutes(),
      date.getSeconds(),
      date.getMilliseconds(),
    );

    return new DateOnly(dt, CultureInfo.currentCulture.calendar);
  }
}

export const XOBIN_DATE_FORMATS: MatDateFormats = {
  parse: {
    dateInput: null,
  },
  display: {
    monthLabel: 'MMMM',
    dateInput: 'yyyy-MM-dd',
    monthYearLabel: 'MMMM yyyy',
    dateA11yLabel: 'yyyy-MMMM-dd',
    monthYearA11yLabel: 'yyyy-MMMM-dd',
  },
};

export function provideXobinDateAdapter(): Provider[] {
  return [
    { provide: DateAdapter, useClass: XobinDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: XOBIN_DATE_FORMATS },
  ];
}
