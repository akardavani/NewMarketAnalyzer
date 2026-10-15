import { themeQuartz } from 'ag-grid-community';

/**
 * تم پیش‌فرض گریدها. همهٔ رنگ‌ها و فونت از توکن‌های CSS (src/styles/theme) می‌آیند، پس با تغییر حالت روشن/تیره،
 * پالت یا فونت، گرید بدون رندر مجدد عوض می‌شود. مقدار ثابت (hex/rgb) این‌جا ننویسید.
 */
export const defaultGridTheme = themeQuartz.withParams({
  browserColorScheme: 'inherit',

  spacing: 10,
  fontFamily: 'var(--fd-font-sans)',
  fontSize: '13px',

  accentColor: 'var(--fd-primary)',
  backgroundColor: 'var(--fd-surface)',
  foregroundColor: 'var(--fd-text)',
  borderColor: 'var(--fd-line)',
  chromeBackgroundColor: 'var(--fd-surface-2)',

  headerFontSize: '13px',
  headerFontWeight: 'bold', // وزن واقعی با CSS در styles/vendor/ag-grid.scss از --fd-fw-bold می‌آید
  headerColumnBorder: false,
  headerBackgroundColor: 'var(--fd-surface-2)',
  headerTextColor: 'var(--fd-text-2)',
  headerVerticalPaddingScale: 0.65,
  headerColumnResizeHandleHeight: '65%',

  wrapperBorder: false,
  selectCellBorder: false,
  rowVerticalPaddingScale: 0.6,
  cellHorizontalPaddingScale: 0.4,

  oddRowBackgroundColor: 'var(--fd-surface-2)',
  rowHoverColor: 'var(--fd-hover)',
  selectedRowBackgroundColor: 'var(--fd-primary-soft)',

  inputBackgroundColor: 'var(--fd-field)',
  inputTextColor: 'var(--fd-text)',
  menuBackgroundColor: 'var(--fd-surface)',
  menuTextColor: 'var(--fd-text)'
});
