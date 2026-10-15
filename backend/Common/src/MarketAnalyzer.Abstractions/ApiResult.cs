using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class ApiResult<T>
    {
        public ApiResult()
        {
            Errors = new List<string>();
        }

        public bool Successed { get; init; }

        public IEnumerable<string> Errors { get; init; }

        public T Data { get; init; }
    }

    public static class ApiResult
    {
        public static readonly ApiResult<object> Success = new()
        {
            Successed = true
        };

        public static ApiResult<T> From<T>(T data)
        {
            return new ApiResult<T>
            {
                Successed = true,
                Data = data
            };
        }

        public static ApiResult<T> FromErrors<T>(IEnumerable<string> errors)
        {
            return new ApiResult<T>
            {
                Successed = false,
                Errors = errors
            };
        }

        public static ApiResult<T> FromError<T>(string error)
        {
            return FromErrors<T>(new[] { error });
        }

        public static ApiResult<object> FromError(string error)
        {
            return new ApiResult<object>
            {
                Successed = false,
                Errors = new[] { error }
            };
        }
    }
}
