const { env } = require("process");

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(";")[0]
    : "https://localhost:7213";

const PROXY_CONFIG = [
  {
    context: [
      "/captcha",
      //"/env.js",
      "/api/Account/Login",
      "/api/Account/ForgotPassword",
      "/api/Account/ResetPassword",
    ],
    target,
    secure: false,
  },
];

module.exports = PROXY_CONFIG;
