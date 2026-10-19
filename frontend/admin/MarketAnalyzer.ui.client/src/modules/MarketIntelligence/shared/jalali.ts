// Jalali <-> Gregorian (jalaali-js algorithm)
export const div = (a: number, b: number): number => Math.trunc(a / b);
export const g2j = (gy: number, gm: number, gd: number): [number, number, number] => {
  const gdm = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334];
  const gy2 = gm > 2 ? gy + 1 : gy;
  let days = 355666 + 365 * gy + div(gy2 + 3, 4) - div(gy2 + 99, 100) + div(gy2 + 399, 400) + gd + gdm[gm - 1];
  let jy = -1595 + 33 * div(days, 12053);
  days %= 12053;
  jy += 4 * div(days, 1461);
  days %= 1461;
  if (days > 365) { jy += div(days - 1, 365); days = (days - 1) % 365; }
  const jm = days < 186 ? 1 + div(days, 31) : 7 + div(days - 186, 30);
  const jd = 1 + (days < 186 ? days % 31 : (days - 186) % 30);
  return [jy, jm, jd];
};
export const j2g = (jy: number, jm: number, jd: number): [number, number, number] => {
  jy += 1595;
  let days = -355668 + 365 * jy + div(jy, 33) * 8 + div((jy % 33) + 3, 4) + jd + (jm < 7 ? (jm - 1) * 31 : (jm - 7) * 30 + 186);
  let gy = 400 * div(days, 146097);
  days %= 146097;
  if (days > 36524) {
    gy += 100 * div(--days, 36524);
    days %= 36524;
    if (days >= 365) days++;
  }
  gy += 4 * div(days, 1461);
  days %= 1461;
  if (days > 365) { gy += div(days - 1, 365); days = (days - 1) % 365; }
  let gd = days + 1;
  const feb = (gy % 4 === 0 && gy % 100 !== 0) || gy % 400 === 0 ? 29 : 28;
  const md = [0, 31, feb, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
  let gm = 0;
  while (gm < 13 && gd > md[gm]) gd -= md[gm++];
  return [gy, gm, gd];
};
export const pad2 = (n: number): string => String(n).padStart(2, '0');
export const toJalaliText = (d: Date): string => {
  const [y, m, dd] = g2j(d.getFullYear(), d.getMonth() + 1, d.getDate());
  return `${y}/${pad2(m)}/${pad2(dd)}`;
};
export const normDigits = (s: string): string =>
  s.replace(/[۰-۹]/g, c => String(c.charCodeAt(0) - 1776)).replace(/[٠-٩]/g, c => String(c.charCodeAt(0) - 1632));
/** Jalali 'yyyy/MM/dd' -> Gregorian 'yyyy-MM-dd', or null if invalid. */
export const jalaliTextToIso = (text: string): string | null => {
  const m = /^(\d{4})[\/\-.](\d{1,2})[\/\-.](\d{1,2})$/.exec(normDigits(text.trim()));
  if (!m) return null;
  const [jy, jm, jd] = [+m[1], +m[2], +m[3]];
  if (jm < 1 || jm > 12 || jd < 1 || jd > 31) return null;
  const [gy, gm, gd] = j2g(jy, jm, jd);
  return `${gy}-${pad2(gm)}-${pad2(gd)}`;
};

export const daysAgo = (days: number): string => {
  const d = new Date();
  d.setDate(d.getDate() - days);
  return toJalaliText(d);
};
export const today = (): string => toJalaliText(new Date());
