// بدون وابستگی: node tools/check-theme-colors.mjs
// رنگ ثابت (hex)، --bs-gray/white/black خام و وزن فونت ثابت (bold/600/700) را در SCSS کامپوننت‌ها و styles ممنوع می‌کند.
// استثنا: src/styles/theme/** (تعریف توکن‌ها) و src/styles/base/variables.scss (تنظیمات build بوت‌استرپ).
import { readdirSync, readFileSync, statSync } from 'node:fs';
import { join, relative, sep } from 'node:path';

const root = join(process.cwd(), 'src');
const allow = ['styles/theme/', 'styles/base/variables.scss'];
const rules = [
  { re: /#[0-9a-fA-F]{3,8}\b/, msg: 'hex ثابت؛ از توکن --fd-* استفاده کنید' },
  { re: /var\(--bs-(?:gray-\d+|white|black)\)/, msg: '--bs-gray/white/black خام؛ از --fd-* استفاده کنید' },
  // @font-face در styles.scss نیاز به مقدار واقعی دارد و مستثناست
  { re: /font-weight:\s*(?:bold|bolder|[5-9]00)\b/, msg: 'وزن ثابت؛ از var(--fd-fw-bold) یا var(--fd-fw-semi) استفاده کنید', skip: ['styles.scss'] }
];

const files = [];
(function walk(dir) {
  for (const name of readdirSync(dir)) {
    const p = join(dir, name);
    if (statSync(p).isDirectory()) walk(p);
    else if (p.endsWith('.scss')) files.push(p);
  }
})(root);

let errors = 0;
for (const file of files) {
  const rel = relative(root, file).split(sep).join('/');
  if (allow.some(a => rel.startsWith(a))) continue;
  const text = readFileSync(file, 'utf8').replace(/\/\*[\s\S]*?\*\//g, m => m.replace(/[^\n]/g, ' '));
  text.split(/\r?\n/).forEach((line, i) => {
    const code = line.replace(/\/\/.*$/, '');
    for (const { re, msg, skip } of rules) {
      if (skip?.includes(rel)) continue;
      if (re.test(code)) { console.error(`${rel}:${i + 1}  ${msg}\n    ${line.trim()}`); errors++; }
    }
  });
}
console.log(errors ? `\n${errors} مورد` : 'OK — رنگ ثابتی در SCSS نیست');
process.exit(errors ? 1 : 0);
