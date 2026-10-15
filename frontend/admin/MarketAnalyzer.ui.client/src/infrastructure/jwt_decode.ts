import { PlainObject } from "./typings";

export interface JwtDecodeOptions {
  header?: boolean;
}

class InvalidTokenError extends Error {
  constructor(message: string) {
    super(message);
  }
}

function b64DecodeUnicode(str: string) {
  return decodeURIComponent(atob(str).replace(/(.)/g, function (_m, p) {
    var code = p.charCodeAt(0).toString(16).toUpperCase();
    if (code.length < 2) {
      code = '0' + code;
    }
    return '%' + code;
  }));
}

function base64_url_decode(str: string): string {
  let output = str.replace(/-/g, "+").replace(/_/g, "/");

  switch (output.length % 4) {
    case 0:
      break;
    case 2:
      output += "==";
      break;
    case 3:
      output += "=";
      break;
    default:
      throw "Illegal base64url string!";
  }

  try {
    return b64DecodeUnicode(output);
  } catch (err) {
    return atob(output);
  }
}

export function jwtDecode<T = PlainObject>(token: string, options?: JwtDecodeOptions): T {
  if (typeof token !== 'string') {
    throw new InvalidTokenError('Invalid token specified');
  }

  options = options || {};
  var pos = options.header === true ? 0 : 1;
  try {
    return JSON.parse(base64_url_decode(token.split('.')[pos]));
  }
  catch (e: any) {
    throw new InvalidTokenError('Invalid token specified: ' + e.message);
  }
}
