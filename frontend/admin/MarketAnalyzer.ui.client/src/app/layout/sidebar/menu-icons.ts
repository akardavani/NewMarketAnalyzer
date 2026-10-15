/**
 * آیکن منوی اصلی (سطح اول). تنها جای تعیین آیکن‌ها همین فایل است؛ زیرمنوها آیکن ندارند.
 * ترتیب مهم است: اولین قاعدهٔ مطابق برنده می‌شود. اگر هیچ‌کدام نخورد، آیکن سرور (در صورت وجود) و در نهایت DEFAULT_ICON.
 * نام آیکن‌ها از فونت پروژه (src/styles/base/icons.scss، کلاس‌های pa-*) است.
 */
const RULES: ReadonlyArray<readonly [RegExp, string]> = [
  [/داشبورد|میزکار|خانه/, 'pa-home-page'],
  [/حسابدار|سند|تراز|دفتر/, 'pa-calculator'],
  [/بورس|معامل|سهام|اوراق|کارگزار/, 'pa-bull'],
  [/نقد|پرداخت|دریافت|بانک|تسویه|وجه/, 'pa-paper-money'],
  [/صندوق|دارایی|سرمایه|ترکیب|nav/i, 'pa-pie-chart'],
  [/مشتری|سرمایه\s?گذار|crm/i, 'pa-user'],
  [/کاربر|دسترسی|نقش|عضویت/, 'pa-settings-profile'],
  [/محتوا|پیام|اطلاع|خبر|سایت/, 'pa-message'],
  [/متولی|نظارت|حسابرس/, 'pa-shield'],
  [/گزارش|آمار|تحلیل/, 'pa-statistics'],
  [/تنظیم|پیکربندی/, 'pa-settings'],
  [/تاریخچه|ورود|خروج/, 'pa-history']
];

const DEFAULT_ICON = 'pa-options';

function normalize(value: string): string {
  return value.replace(/ي/g, 'ی').replace(/ك/g, 'ک').replace(/\u200c/g, '');
}

/** کلاس کامل آیکن (مثلاً «pa pa-calculator») برای یک آیتم سطح اول. */
export function pickMenuIcon(label: string, serverIcon?: string): string {
  const text = normalize(label ?? '');
  const hit = RULES.find(([re]) => re.test(text));
  if (hit) {
    return `pa ${hit[1]}`;
  }
  return serverIcon?.trim() || `pa ${DEFAULT_ICON}`;
}
