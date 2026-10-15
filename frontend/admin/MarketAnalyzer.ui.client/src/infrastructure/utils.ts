import { HttpParams, HttpResponse } from '@angular/common/http';
import { Func, Nullable, NumericKeys, PlainObject } from '@typings';
import { CultureInfo, DateTime, DateTimeFormatInfo, DateTimeKind } from '@xobin/globalization';

const PLACE_HOLDER_RX = /{(\d+)}/g;
export function formatString(format: string, ...args: any[]): string {
  return format
    .replace(PLACE_HOLDER_RX, (match: string, index: number) => (typeof args[index] !== undefined) ? args[index] : match);
}

export function deepClone<T = any>(obj: any): T {
  return structuredClone<T>(obj);
}

export function simpleHashCode(str: string): number {
  let hash = 0;
  for (let i = 0; i < str.length; i++) {
    hash = (hash * 31 + str.charCodeAt(i)) | 0;
  }
  return hash;
}

export function groupBy<TEntity, TKey>(
  source: TEntity[],
  keySelector: (e: TEntity) => TKey
): Map<TKey, TEntity[]> {
  const map = new Map<TKey, TEntity[]>();
  for (const item of source) {
    const key = keySelector(item);
    const hashKey = JSON.stringify(key);
    let group = Array.from(map.keys()).find(exists => JSON.stringify(exists) === hashKey);
    if (group === undefined) { 
      group = key;
      map.set(group, []);
    }

    map.get(group)!.push(item);
  }
  return map;
}

export type AbstractConstructor<T = {}> = abstract new (...args: any[]) => T;

export function Mixin<TBase extends AbstractConstructor>(Base: TBase): AbstractConstructor<TBase> {
  return (class extends (Base as any) {

  }) as any;
}


export function bind(): MethodDecorator {
  return function <T>(target: Object, propertyKey: string | symbol, descriptor: TypedPropertyDescriptor<T>): TypedPropertyDescriptor<T> {
    let fn = descriptor.value;
    if (typeof descriptor.value !== 'function') {
      throw new TypeError(`@bind decorator can only be applied to methods not: ${typeof fn}`);
    }

    let definingProperty = false;

    return {
      enumerable: true,
      configurable: true,
      get() {
        if (definingProperty || this === (target as any).prototype || this.hasOwnProperty(propertyKey) || typeof fn !== 'function') {
          return fn;
        }

        const boundFn = fn.bind(this);
        definingProperty = true;
        Object.defineProperty(this, propertyKey, {
          configurable: true,
          get: () => boundFn,
          set(value) {
            fn = value;
            delete this[propertyKey];
          }
        });
        definingProperty = false;
        return boundFn;
      },
      set(value) { fn = value; }
    };
  };
}

export function getMemberValue(obj: any, memberName: string | null): any {
  if (!memberName) {
    return obj;
  }

  return obj[memberName];
}

export function properties<T>(obj: T): Array<Extract<keyof T, string>> {
  const keys: any = Object.keys(obj as any);
  return keys;
}

export function getValue<TModel extends object, K extends keyof TModel>(model: TModel, property: K): TModel[K] {
  return (model as any)[`__${String(property)}__`];
}

export function getOrCreateValue<TModel extends object, K extends keyof TModel>(model: TModel, property: K, factory: () => TModel[K]): TModel[K] {
  const field = `__${String(property)}__`;
  if (!(field in model)) {
    (model as any)[field] = factory();
  }

  return (model as any)[field];
}

export function setValue<TModel extends object, K extends keyof TModel>(model: TModel, property: K, value: TModel[K]): void {
  (model as any)[`__${String(property)}__`] = value;
}

export function providedInRoot(obj: object): boolean {
  const meta = (obj.constructor as any)['ɵprov'];
  return meta && meta.providedIn === 'root';
}

//#endregion

//#region [Http]

export function replacePath(query?: PlainObject): void {
  const path = buildUrl(`${window.location.origin + window.location.pathname}`, query);
  window.history.pushState({ path: path }, '', path);
}

export function buildUrl(path: string, obj: any): string {
  const query = params(obj);
  return `${path}?${query.params.toString()}`;
}

/**
 * Create an instance of HttpParams by given data.
 * IMPORTANT: Don't use in `POST` or `PUT` JSON requests.
 * @param data The data object to serialize as query string or form data
 */
function params(data: any): { params: HttpParams } {
  return {
    params: data ?
      Object.keys(data).reduce((httpParams, key) => appendParam(httpParams, key, data[key]), new HttpParams()) : new HttpParams()
  };
}

function appendParam(params: HttpParams, key: string, value: any): HttpParams {
  const type = typeof value;

  if (value === 'null') {
    throw new Error('It seems that the value of a form field is bind with [value] binding, which should be changed to [ngValue] binding');
  }

  if (value === null || value === undefined) {
    return params;
  }
  else if (Array.isArray(value)) {
    return value.reduce((collection, itemValue, index) => collection.append(`${key}[${index}]`, itemValue), params);
  }
  else if (value instanceof Date || value instanceof DateTime) {
    return params.append(key, formatDateTime(value, 'yyyy-MM-dd', CultureInfo.invariantCulture));
  }
  else if (type == 'bigint' || type == 'boolean' || type == 'number' || type == 'string') {
    return params.append(key, value);
  }
  else {
    const parentProperty = key;
    return Object
      .keys(value)
      .reduce((httpParams, property) => appendParam(httpParams, `${parentProperty}.${property}`, value[property]), params)
  }
}

export function getQuery(url?: string): Nullable<PlainObject<string>> {
  if (!url) {
    url = window.location.href;
  }

  const queryIndex = url.indexOf('?');
  if (queryIndex == -1) {
    return null;
  }

  const query = url.substring(queryIndex + 1)
    .split('&')
    .filter(q => !!q)
    .reduce((pairs, q) => {
      const results = q.split('=');
      pairs[results[0]] = results[1] ? decodeURIComponent(results[1].replace(/\+/g, ' ')) : '';
      return pairs;
    }, ({} as PlainObject<string>));

  return query;
}

export function getQueryValue(key: string, url?: string): string | null {
  const query = getQuery(url);
  if (!query) {
    return null;
  }
  return query[key];
}

export function queryValueOrDefault<T>(query: Nullable<PlainObject<string>>, key: string, defaultValue: T): string | T;
export function queryValueOrDefault<T>(query: Nullable<PlainObject<string>>, key: string, defaultValue: T, binder: Func<string, T>): T;
export function queryValueOrDefault<T>(query: Nullable<PlainObject<string>>, key: string, defaultValue: T, binder?: Func<string, T>): string | T {
  if (!query || !Object.hasOwn(query, key)) {
    return defaultValue;
  }

  return binder ? binder(query[key]) : query[key];
}

export function apiUrl(path: string): string {
  return appSettings.apiUrl + path;
}

export function saveFile(response: HttpResponse<any>): void {
  const blob = new Blob([response.body], { type: response.headers.get('Content-Type') || '' });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = response.headers.get('X-File-Name') || 'file';
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
}

//#endregion

//#region [ToggleHeight]

export function toggleHeight(elem: HTMLElement): void {
  if (elem.style.height == 'auto') {
    elem.style.height = elem.scrollHeight + 'px';
  }

  if (elem.clientHeight >= elem.scrollHeight) {
    elem.style.height = '0px';
  }
  else {
    elem.style.height = elem.scrollHeight + 'px';
    elem.addEventListener('transitionend', setHeightToAuto)
  }
}

function setHeightToAuto(ev: TransitionEvent): void {
  (ev.target as HTMLElement).style.height = 'auto';
  (ev.target as HTMLElement).removeEventListener('transitionend', setHeightToAuto);
}

//#endregion

//#region [DateTimeFormat]

enum DatePart {
  None,
  Year,
  Month,
  Day,
  Hour,
  Minute,
  Second,
  Millisecond,
  Designator,
  TimeSeparator,
  DateSeparator
}

interface DatePartEntry {
  pattern: string;
  part: DatePart;
  parser: DatePartParser;
}

type DatePartParser = (value: string, context: DateContext) => any;

interface DateContext {
  year?: DateSection;
  month?: DateSection;
  day?: DateSection;
  hour?: DateSection;
  minute?: DateSection;
  second?: DateSection;
  millisecond?: DateSection;
  designator?: DateSection;
  culture: CultureInfo;
}

interface DateSection {
  value: string;
  parser: DatePartParser;
}

const LONG_YEAR_RX = /y{5,}/;
const DATE_FORMAT_RX = /(y{5,}|yyyy|yyy|yy|y|M{4,}|MMM|MM|M|d{4,}|ddd|dd|d|h{2,}|h|H{2,}|H|m{2,}|m|s{2,}|s|fffffff|ffffff|fffff|ffff|fff|ff|f|FFFFFFF|FFFFFF|FFFFF|FFFF|FFF|FF|F|t{2,}|t|z{3,}|zz|z|K|g|:|\/)/;

function escapeRegex(value: string): string {
  return value.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
}

function dateNumberParser(value: string, context: DateContext): number {
  return parseInt(value, 10);
}

function monthNamesParser(value: string, context: DateContext): number {
  const index = context.culture.dateTimeFormat.monthNames.indexOf(value);
  if (index == -1) {
    throw new Error(`Invalid month name: [${value}]`);
  }
  return index + 1;
}

function abbreviatedMonthNamesParser(value: string, context: DateContext): number {
  const index = context.culture.dateTimeFormat.abbreviatedMonthNames.indexOf(value);
  if (index == -1) {
    throw new Error(`Invalid abbreviated month name: [${value}]`);
  }
  return index + 1;
}

function hour12Parser(value: string, context: DateContext): number {
  const hour = parseInt(value, 10);
  if (hour < 0 || hour > 12) {
    throw new Error(`Out of range hour: [${hour}].`);
  }
  if (!context.designator) {
    return hour;
  }

  const addition = context.designator.value == context.culture.dateTimeFormat.pmDesignator ? 12 : 0;
  return hour + addition;
}

function hour24Parser(value: string, context: DateContext): number {
  const hour = parseInt(value, 10);
  if (hour < 0 || hour > 23) {
    throw new Error(`Out of range hour: [${hour}].`);
  }

  if (context.designator &&
    ((hour <= 12 && context.designator.value == context.culture.dateTimeFormat.pmDesignator) ||
      (hour > 12 && context.designator.value == context.culture.dateTimeFormat.amDesignator))) {
    throw new Error(`Invalid time designator: [hour: ${hour}, designator: ${context.designator.value}]`);
  }
  return hour;
}

function minuteParser(value: string, context: DateContext): number {
  const num = parseInt(value, 10);
  if (num < 0 || num > 59) {
    throw new Error(`Out of range minute/second: [${num}]`);
  }
  return num;
}

function noopDatePartParser(value: string, context: DateContext): string {
  return value;
}

export function parseDateString(value: string, format: string, culture = CultureInfo.invariantCulture): Date | null {
  if (!value) {
    return null;
  }

  const entries: PlainObject<DatePartEntry> = {
    'yyyy': { pattern: '([0-9]{4})', part: DatePart.Year, parser: dateNumberParser },
    'yyy': { pattern: '([0-9]{4})', part: DatePart.Year, parser: dateNumberParser },
    'yy': { pattern: '([0-9]{2})', part: DatePart.Year, parser: dateNumberParser },
    'y': { pattern: '([0-9]{1,2})', part: DatePart.Year, parser: dateNumberParser },
    //..........................................................................................
    'MMMM': { pattern: `(${culture.dateTimeFormat.monthNames.map(escapeRegex).join('|')})`, part: DatePart.Month, parser: monthNamesParser },
    'MMM': { pattern: `(${culture.dateTimeFormat.abbreviatedMonthNames.map(escapeRegex).join('|')})`, part: DatePart.Month, parser: abbreviatedMonthNamesParser },
    'MM': { pattern: '([0-9]{2})', part: DatePart.Month, parser: dateNumberParser },
    'M': { pattern: '([0-9]{1,2})', part: DatePart.Month, parser: dateNumberParser },
    //..........................................................................................
    'dddd': { pattern: `(${culture.dateTimeFormat.dayNames.map(escapeRegex).join('|')})`, part: DatePart.Day, parser: noopDatePartParser },
    'ddd': { pattern: `(${culture.dateTimeFormat.abbreviatedDayNames.map(escapeRegex).join('|')})`, part: DatePart.Day, parser: noopDatePartParser },
    'dd': { pattern: '([0-9]{2})', part: DatePart.Day, parser: dateNumberParser },
    'd': { pattern: '([0-9]{1,2})', part: DatePart.Day, parser: dateNumberParser },
    //..........................................................................................
    'hh': { pattern: '([0-9]{2})', part: DatePart.Hour, parser: hour12Parser },
    'h': { pattern: '([0-9]{1,2})', part: DatePart.Hour, parser: hour12Parser },
    //..........................................................................................
    'HH': { pattern: '([0-9]{2})', part: DatePart.Hour, parser: hour24Parser },
    'H': { pattern: '([0-9]{1,2})', part: DatePart.Hour, parser: hour24Parser },
    //..........................................................................................
    'mm': { pattern: '([0-9]{2})', part: DatePart.Minute, parser: minuteParser },
    'm': { pattern: '([0-9]{1,2})', part: DatePart.Minute, parser: minuteParser },
    //..........................................................................................
    'ss': { pattern: '([0-9]{2})', part: DatePart.Second, parser: minuteParser },
    's': { pattern: '([0-9]{1,2})', part: DatePart.Second, parser: minuteParser },
    //..........................................................................................
    'fff': { pattern: '([0-9]{3})', part: DatePart.Millisecond, parser: dateNumberParser },
    'ff': { pattern: '([0-9]{2})', part: DatePart.Millisecond, parser: dateNumberParser },
    'f': { pattern: '([0-9]{1})', part: DatePart.Millisecond, parser: dateNumberParser },
    //..........................................................................................
    'tt': { pattern: `(${culture.dateTimeFormat.amDesignator}|${culture.dateTimeFormat.pmDesignator})`, part: DatePart.Designator, parser: noopDatePartParser },
    't': { pattern: `(${culture.dateTimeFormat.amDesignator[0]}|${culture.dateTimeFormat.pmDesignator[0]})`, part: DatePart.Designator, parser: noopDatePartParser },
    //..........................................................................................
    ':': { pattern: `(\\${culture.dateTimeFormat.timeSeparator})`, part: DatePart.TimeSeparator, parser: noopDatePartParser },
    '/': { pattern: `(\\${culture.dateTimeFormat.dateSeparator})`, part: DatePart.DateSeparator, parser: noopDatePartParser }
  };

  const keyParts: DatePartEntry[] = [];
  const pattern = format
    .split(DATE_FORMAT_RX)
    .map(part => {
      if (part == '') {
        return part;
      }

      let entry: DatePartEntry;

      if (LONG_YEAR_RX.test(part)) {
        entry = {
          part: DatePart.Year,
          pattern: `([0-9]{${part.length}})`,
          parser: dateNumberParser
        };
      }
      else if (Object.hasOwn(entries, part)) {
        entry = entries[part];
      }
      else {
        entry = {
          part: DatePart.None,
          pattern: `(${escapeRegex(part).replace(/\s+/g, '\\s+')})`,
          parser: noopDatePartParser
        };
      }

      keyParts.push(entry);
      return entry.pattern;
    })
    .join('');

  const rx = new RegExp(`^${pattern}$`);
  const match = rx.exec(value);
  if (!match) {
    throw new Error(`The string '${value}' was not recognized as a valid DateTime.`);
  }

  const context: DateContext = { culture: culture };

  match.forEach((value, index) => {
    if (index == 0) {
      return;
    }

    const entry = keyParts[index - 1];
    switch (entry.part) {
      case DatePart.Year:
        context.year = { value: value, parser: entry.parser };
        return;

      case DatePart.Month:
        context.month = { value: value, parser: entry.parser };
        return;

      case DatePart.Day:
        context.day = { value: value, parser: entry.parser };
        return;

      case DatePart.Hour:
        context.hour = { value: value, parser: entry.parser };
        return;

      case DatePart.Minute:
        context.minute = { value: value, parser: entry.parser };
        return;

      case DatePart.Second:
        context.second = { value: value, parser: entry.parser };
        return;

      case DatePart.Millisecond:
        context.millisecond = { value: value, parser: entry.parser };
        return;

      case DatePart.Designator:
        context.designator = { value: value, parser: entry.parser };
        return;
    }
  });

  return buildDate(context);
}

function buildDate(context: DateContext): Date {
  const now = DateTime.now;
  const year = context.year ? context.year.parser(context.year.value, context) : context.culture.calendar.getYear(now);
  const month = context.month ? context.month.parser(context.month.value, context) : context.culture.calendar.getMonth(now);
  const day = context.day ? context.day.parser(context.day.value, context) : 1;
  const hour = context.hour ? context.hour.parser(context.hour.value, context) : 0;
  const minute = context.minute ? context.minute.parser(context.minute.value, context) : 0;
  const second = context.second ? context.second.parser(context.second.value, context) : 0;
  const millisecond = context.millisecond ? context.millisecond.parser(context.millisecond.value, context) : 0;

  const anyTimePart = !context.hour || !context.minute || !context.second || !context.millisecond;

  const dateTime = context.culture.calendar
    .toDateTime(
      year,
      context.month ? month : anyTimePart ? month : 1,
      day,
      hour,
      minute,
      second,
      millisecond);

  return new Date(
    dateTime.year,
    dateTime.month - 1,
    dateTime.day,
    dateTime.hour,
    dateTime.minute,
    dateTime.second,
    dateTime.millisecond);
}

(window as any).parseDateString = parseDateString;

export function convertDates(obj: any): any {
  if (!obj) return obj;

  if (obj instanceof Date) {
    return toLocalDateString(obj);
  }

  if (Array.isArray(obj)) {
    return obj.map(o => convertDates(o));
  }

  if (typeof obj === 'object') {
    const newObj: any = {};
    for (const key of Object.keys(obj)) {
      newObj[key] = convertDates(obj[key]);
    }
    return newObj;
  }

  return obj;
}

function toLocalDateString(date: Date): string {
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export function parseDateTime(value: string | Date | null | undefined): DateTime | null {
  if (!value) {
    return null;
  }

  if (typeof (value) === 'string') {
    return parseDateTime(new Date(value));
  }

  if (isNaN(value.getTime())) {
    return null;
  }

  return new DateTime(
    value.getFullYear(),
    value.getMonth() + 1,
    value.getDate(),
    value.getHours(),
    value.getMinutes(),
    value.getSeconds(),
    value.getMilliseconds(),
    DateTimeKind.local);
}

(window as any).parseDateTime = parseDateTime;

export function getFirstDateOfYear(date: Date): Date {
  const dateTime = parseDateTime(date);
  const calendar = CultureInfo.currentCulture.calendar;
  const nativeYear = calendar.getYear(dateTime!);
  const firstDate = calendar.toDateTime(nativeYear, 1, 1, 0, 0, 0, 0);
  return new Date(firstDate.year, firstDate.month - 1, firstDate.day);
}

export function toDate(date: DateTime): Date {
  return new Date(date.year, date.month - 1, date.day);
}

export function getLastDateOfYear(date: Date): Date {
  const dateTime = parseDateTime(date);
  const calendar = CultureInfo.currentCulture.calendar;
  const nativeYear = calendar.getYear(dateTime!);
  const nativeDay = calendar.isLeapYear(nativeYear) ? 30 : 29;
  const lastDate = calendar.toDateTime(nativeYear, 12, nativeDay, 0, 0, 0, 0);
  return new Date(lastDate.year, lastDate.month - 1, lastDate.day);
}

const RoundtripFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

export function formatDateTime(date: DateTime | Date | string | null | undefined, format: string, culture: CultureInfo = CultureInfo.currentCulture): string {
  if (!date) {
    return '';
  }

  if (date instanceof Date || typeof date == 'string') {
    return formatDateTime(parseDateTime(date), format, culture);
  }

  if (format.length == 1) {
    format = getRealFormat(format, culture.dateTimeFormat);
  }

  let year: number;
  let month: number;
  let day: number;
  //let dayOfWeek: number;
  let hour: number;
  let minute: number;
  let second: number;
  let millisecond: number;

  const dateOnly = date.date;

  const calendar = culture.calendar;
  year = calendar.getYear(dateOnly);
  month = calendar.getMonth(dateOnly);
  day = calendar.getDayOfMonth(dateOnly);
  //dayOfWeek = calendar.getDayOfWeek(this);
  hour = calendar.getHour(date);
  minute = calendar.getMinute(date);
  second = calendar.getSecond(date);
  millisecond = calendar.getMilliseconds(date);

  const dateTimeFormat = CultureInfo.currentCulture.dateTimeFormat;

  const data: PlainObject<any> = {};
  data["yyyy"] = year;
  data["yyy"] = year;
  data["yy"] = pad((year % 100));
  data["y"] = (year % 100);
  //..........................................................................................
  data["MMMM"] = dateTimeFormat.monthNames[month - 1];
  data["MMM"] = dateTimeFormat.abbreviatedMonthNames[month - 1];
  data["MM"] = pad(month);
  data["M"] = month;
  //..........................................................................................
  data["dddd"] = dateTimeFormat.dayNames[date.dayOfWeek];
  data["ddd"] = dateTimeFormat.shortestDayNames[date.dayOfWeek];
  data["dd"] = pad(day);
  data["d"] = day;
  //..........................................................................................
  data["hh"] = pad(hour > 12 ? hour - 12 : hour);
  data["h"] = hour > 12 ? hour - 12 : hour;
  //..........................................................................................
  data["HH"] = pad(hour);
  data["H"] = hour;
  //..........................................................................................
  data["mm"] = pad(minute);
  data["m"] = minute;
  //..........................................................................................
  data["ss"] = pad(second);
  data["s"] = second;
  //..........................................................................................
  data["fffffff"] = "";
  data["ffffff"] = "";
  data["fffff"] = "";
  data["ffff"] = "";
  data["fff"] = millisecond;
  data["ff"] = Math.round(millisecond / 10);
  data["f"] = Math.round(millisecond / 100);
  //..........................................................................................
  data["FFFFFFF"] = "";
  data["FFFFFF"] = "";
  data["FFFFF"] = "";
  data["FFFF"] = "";
  data["FFF"] = "";
  data["FF"] = "";
  data["F"] = "";
  //..........................................................................................
  data["tt"] = (hour > 12 ? dateTimeFormat.pmDesignator : dateTimeFormat.amDesignator);
  data["t"] = (hour > 12 ? dateTimeFormat.pmDesignator : dateTimeFormat.amDesignator)[0]; // اینجا فقط یک کارکتر نیاز داریم
  //..........................................................................................
  data["zzz"] = "";
  data["zz"] = "";
  data["z"] = "";
  //..........................................................................................
  data["K"] = "";
  //..........................................................................................
  data["g"] = "";
  //..........................................................................................
  data[":"] = dateTimeFormat.timeSeparator;
  data["/"] = dateTimeFormat.dateSeparator;
  //..........................................................................................

  let output = "";
  const res = format.split(DATE_FORMAT_RX);

  for (let i = 0; i < res.length; i++) {
    if (res[i]) {
      if (data[res[i]]) {
        output += data[res[i]];
      }
      else {
        output += res[i];
      }
    }
  }

  return output;
}

function getRealFormat(format: string, dtfi: DateTimeFormatInfo): string {
  let realFormat: string;

  switch (format[0]) {
    case 'd':       // Short Date
      realFormat = dtfi.shortDatePattern;
      break;
    case 'D':       // Long Date
      realFormat = dtfi.longDatePattern;
      break;
    case 'f':       // Full (long date + short time)
      realFormat = dtfi.longDatePattern + " " + dtfi.shortTimePattern;
      break;
    case 'F':       // Full (long date + long time)
      realFormat = dtfi.fullDateTimePattern;
      break;
    case 'g':       // General (short date + short time)
      realFormat = dtfi.shortDatePattern + dtfi.shortTimePattern;
      break;
    case 'G':       // General (short date + long time)
      realFormat = dtfi.shortDatePattern + dtfi.longTimePattern;
      break;
    case 'm':
    case 'M':       // Month/Day Date
      realFormat = dtfi.monthDayPattern;
      break;
    case 'o':
    case 'O':
      realFormat = RoundtripFormat;
      break;
    case 'r':
    case 'R':       // RFC 1123 Standard
      realFormat = dtfi.rfC1123Pattern;
      break;
    case 's':       // Sortable without Time Zone Info
      realFormat = dtfi.sortableDateTimePattern;
      break;
    case 't':       // Short Time
      realFormat = dtfi.shortTimePattern;
      break;
    case 'T':       // Long Time
      realFormat = dtfi.longTimePattern;
      break;
    case 'u':       // Universal with Sortable format
      realFormat = dtfi.universalSortableDateTimePattern;
      break;
    case 'U':       // Universal with Full (long date + long time) format
      realFormat = dtfi.fullDateTimePattern;
      break;
    case 'y':
    case 'Y':       // Year/Month Date
      realFormat = dtfi.yearMonthPattern;
      break;
    default:
      realFormat = format;
      break;
  }

  return realFormat;
}

function pad(value: number): string {
  return (value > 9) ?
    "" + value :
    "0" + value;
}

//#endregion

//#region [Numbers]

const NUMBER_FORMAT = Intl.NumberFormat('en', {
  maximumFractionDigits: 0,
  minimumFractionDigits: 0
});

export function formatCurrency(value: number): string {
  //TODO: Check value is valid
  return NUMBER_FORMAT.format(value);
}

export function formatNegative(value: number): string {
  if (value >= 0) {
    return formatCurrency(value);
  }

  return `(${formatCurrency(Math.abs(value))})`;
}

export function sumOf<T, K extends NumericKeys<T>>(data: T[], key: K, initialValue: number = 0): number {
  return data.reduce((prev, cur) => prev + (cur[key] as number), initialValue);
}

//#endregion
