using MarketAnalyzer.Abstractions;

namespace System
{
    public static class ExceptionExtensions
    {
        public static FlattedException ToFlat(this Exception exception)
        {
            if (exception is null)
            {
                return null;
            }

            return new FlattedException()
            {
                Source = exception.Source,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                InnerException = ToFlat(exception.InnerException)
            };
        }       
    }
}
