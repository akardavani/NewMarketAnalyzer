import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Inject, OnDestroy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { DateAdapter, MAT_DATE_FORMATS, MatDateFormats } from '@angular/material/core';
import { MatCalendar, MatCalendarView } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { Subject, takeUntil } from 'rxjs';
import { DateOnly } from '../xobin-date-adapter';

export const yearsPerPage = 24;

@Component({
  selector: 'calendar-header',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './calendar-header.component.html',
  styleUrl: './calendar-header.component.scss'
})
export class CalendarHeaderComponent implements OnDestroy {

  private _destroyed = new Subject<void>();

  constructor(
    private _calendar: MatCalendar<DateOnly>,
    private _dateAdapter: DateAdapter<DateOnly>,
    @Inject(MAT_DATE_FORMATS) private _dateFormats: MatDateFormats,
    cdr: ChangeDetectorRef) {
    _calendar.stateChanges
      .pipe(takeUntil(this._destroyed))
      .subscribe(() => cdr.detectChanges());
  }

  get monthLabel() {
    return this._dateAdapter
      .format(this._calendar.activeDate, this._dateFormats.display.monthLabel)
      .toLocaleUpperCase();
  }

  get yearLabel() {
    return this._dateAdapter
      .getYear(this._calendar.activeDate);
  }

  showView(view: MatCalendarView): void {
    this._calendar.currentView = view;
  }

  /** Handles user clicks on the previous button. */
  previousClicked(mode: 'month' | 'year') {
    const activeDate = this._calendar.activeDate;
    if (mode == 'month') {
      this._calendar.activeDate = this._dateAdapter.addCalendarMonths(activeDate, -1)
    }
    else {
      this._calendar.activeDate = this._dateAdapter.addCalendarYears(activeDate, this._calendar.currentView == 'multi-year' ? -yearsPerPage : -1)
    }
  }

  /** Handles user clicks on the next button. */
  nextClicked(mode: 'month' | 'year') {
    const activeDate = this._calendar.activeDate;
    if (mode == 'month') {
      this._calendar.activeDate = this._dateAdapter.addCalendarMonths(activeDate, 1);
    }
    else {
      this._calendar.activeDate = this._dateAdapter.addCalendarYears(activeDate, this._calendar.currentView == 'multi-year' ? yearsPerPage : 1)
    }
  }

  /** Whether the previous period button is enabled. */
  previousEnabled(): boolean {
    if (!this._calendar.minDate) {
      return true;
    }
    return (
      !this._calendar.minDate || !this._isSameView(this._calendar.activeDate, this._calendar.minDate)
    );
  }

  /** Whether the next period button is enabled. */
  nextEnabled(): boolean {
    return (
      !this._calendar.maxDate || !this._isSameView(this._calendar.activeDate, this._calendar.maxDate)
    );
  }

  ngOnDestroy() {
    this._destroyed.next();
    this._destroyed.complete();
  }

  /** Whether the two dates represent the same view in the current view mode (month or year). */
  private _isSameView(date1: DateOnly, date2: DateOnly): boolean {
    if (this._calendar.currentView == 'month') {
      return (
        this._dateAdapter.getYear(date1) == this._dateAdapter.getYear(date2) &&
        this._dateAdapter.getMonth(date1) == this._dateAdapter.getMonth(date2)
      );
    }
    if (this._calendar.currentView == 'year') {
      return this._dateAdapter.getYear(date1) == this._dateAdapter.getYear(date2);
    }
    // Otherwise we are in 'multi-year' view.
    return isSameMultiYearView(
      this._dateAdapter,
      date1,
      date2,
      this._calendar.minDate,
      this._calendar.maxDate,
    );
  }

}

function isSameMultiYearView<D>(dateAdapter: DateAdapter<D>, date1: D, date2: D, minDate: D | null, maxDate: D | null): boolean {
  const year1 = dateAdapter.getYear(date1);
  const year2 = dateAdapter.getYear(date2);
  const startingYear = getStartingYear(dateAdapter, minDate, maxDate);
  return (
    Math.floor((year1 - startingYear) / yearsPerPage) ===
    Math.floor((year2 - startingYear) / yearsPerPage)
  );
}

/**
* We pick a "starting" year such that either the maximum year would be at the end
* or the minimum year would be at the beginning of a page.
*/
function getStartingYear<D>(dateAdapter: DateAdapter<D>, minDate: D | null, maxDate: D | null): number {
  let startingYear = 0;
  if (maxDate) {
    const maxYear = dateAdapter.getYear(maxDate);
    startingYear = maxYear - yearsPerPage + 1;
  }
  else if (minDate) {
    startingYear = dateAdapter.getYear(minDate);
  }
  return startingYear;
}
