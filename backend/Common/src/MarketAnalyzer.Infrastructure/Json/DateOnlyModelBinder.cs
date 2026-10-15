using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Json
{
    internal class DateOnlyModelBinder : IModelBinder
    {
        private readonly ILogger _logger;
        public const DateTimeStyles DateStyles = DateTimeStyles.RoundtripKind;

        public DateOnlyModelBinder(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DateOnlyModelBinder>();
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                //logger.FoundNoValueInRequest(bindingContext);

                // no entry
                //_logger.DoneAttemptingToBindModel(bindingContext);
                return Task.CompletedTask;
            }

            var modelState = bindingContext.ModelState;
            modelState.SetModelValue(modelName, valueProviderResult);
            var metadata = bindingContext.ModelMetadata;
            var type = metadata.UnderlyingOrModelType;
            try
            {
                var value = valueProviderResult.FirstValue;

                object model;
                if (string.IsNullOrWhiteSpace(value))
                {
                    // Parse() method trims the value (with common DateTimeSyles) then throws if the result is empty.
                    model = null;
                }
                else if (type == typeof(DateOnly))
                {
                    var date = DateTime.Parse(value, valueProviderResult.Culture, DateTimeStyles.RoundtripKind);
                    model = date.ToDateOnly();
                }
                else
                {
                    throw new NotSupportedException();
                }

                // When converting value, a null model may indicate a failed conversion for an otherwise required
                // model (can't set a ValueType to null). This detects if a null model value is acceptable given the
                // current bindingContext. If not, an error is logged.
                if (model == null && !metadata.IsReferenceOrNullableType)
                {
                    modelState.TryAddModelError(
                        modelName,
                        metadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                            valueProviderResult.ToString()));
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(model);
                }
            }
            catch (Exception exception)
            {
                // Conversion failed.
                modelState.TryAddModelError(modelName, exception, metadata);
            }

            //_logger.DoneAttemptingToBindModel(bindingContext);
            return Task.CompletedTask;
        }
    }
}
