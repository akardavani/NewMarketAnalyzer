using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace MarketAnalyzer.Infrastructure.Json
{
    internal class DateOnlyModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var modelType = context.Metadata.UnderlyingOrModelType;
            if (modelType == typeof(DateOnly))
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new DateOnlyModelBinder(loggerFactory);
            }

            return null;
        }
    }
}
