using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.ApiClient
{
  internal class HttpErrorDelegatingHandler : DelegatingHandler
  {
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var response = await base.SendAsync(request, cancellationToken);
      if (response.StatusCode == HttpStatusCode.InternalServerError)
      {
        var data = new ApiClientResult<object>()
        {
          Successed = false,
          Data = null,
          Errors = ["خطای داخلی سرور: امکان پردازش درخواست شما وجود ندارد."],
        };

        response.StatusCode = HttpStatusCode.OK;
        response.Content = JsonContent.Create(data, mediaType: null, options: null);
      }
      else if (!response.IsSuccessStatusCode)
      {
        response.StatusCode = HttpStatusCode.OK;
      }

      return response;
    }
  }
}
