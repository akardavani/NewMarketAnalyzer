using Microsoft.Extensions.Options;
using Pouya.Fund.IdentityClient.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Pouya.Fund.IdentityClient
{
  public class IdentityApiClient
  {
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals
    };

    public IdentityApiClient(HttpClient client, IOptions<IdentityClientOptions> options)
    {
      _client = client;
      _client.BaseAddress = new Uri(options.Value.Url);
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("api_key", options.Value.ApiKey);
    }

    public async Task<IdentityResult<AuthResult>> LoginAsync(LoginRequest request, CancellationToken cancellation = default)
    {
      var response = await _client
          .PostAsJsonAsync("v1/Account/Login", request, cancellation);

      var responseBody = await response.Content.ReadAsStringAsync(cancellation);

      if (!response.IsSuccessStatusCode)
      {
        // Business errors (e.g. wrong user/password) come back as 400 with an ApiResult body.
        // Return them as a normal result so callers can show the message instead of a 500.
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
          try
          {
            var failed = JsonSerializer.Deserialize<IdentityResult<AuthResult>>(responseBody, _serializerOptions);
            if (failed is { Successed: false, Errors: not null })
            {
              return failed;
            }
          }
          catch (JsonException)
          {
          }
        }

        throw new Exception($"StatusCode: {response.StatusCode}\n{responseBody}");
      }

      return JsonSerializer.Deserialize<IdentityResult<AuthResult>>(responseBody, _serializerOptions);
    }

    public async Task<IdentityResult<int>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellation = default)
    {
      var response = await _client
          .PostAsJsonAsync("v1/Account/ForgotPassword", request, cancellation);

      response.EnsureSuccessStatusCode();

      return await response
          .Content
          .ReadFromJsonAsync<IdentityResult<int>>(_serializerOptions, cancellation);
    }

    public async Task<IdentityResult<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellation = default)
    {
      var response = await _client
          .PostAsJsonAsync("v1/Account/ResetPassword", request, cancellation);

      response.EnsureSuccessStatusCode();

      return await response
          .Content
          .ReadFromJsonAsync<IdentityResult<bool>>(_serializerOptions, cancellation);
    }

  }
}
