using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MarketAnalyzer.Abstractions;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.Use(static async (HttpContext context, RequestDelegate next) =>
            {
                try
                {
                    await next(context);
                }
                catch (ClientExposedException ex)
                {
                    Log(context, ex);
                    await ExecuteAsync(context, ex.Message);
                }
                catch (Exception ex)
                {
                    Log(context, ex);
                    var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
                    var message = environment.IsDevelopment() ?
                        ex.Message :
                        "درخواست شما با خطای داخلی سرور مواجه شد. لطفا پارامترهای ارسالی خود را بررسی نمایید.";
                    await ExecuteAsync(context, message);
                }
            });
        }

        private static Task ExecuteAsync(HttpContext context, string message)
        {
            var actionContext = new ActionContext() { HttpContext = context };
            var result = new BadRequestObjectResult(ApiResult.FromError<object>(message));
            return result.ExecuteResultAsync(actionContext);
        }

        private static void Log(HttpContext context, Exception exception)
        {
            var factory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = factory.CreateLogger("ExceptionHandlerExtensions");
            logger.LogError(exception, "There was an error in request:");
        }
    }
}
