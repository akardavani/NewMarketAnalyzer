using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.SettingsFileProvider
{
  public static class SettingsStaticFileExtensions
  {
    public static IApplicationBuilder UseSettingsFileProvider(this IApplicationBuilder app)
    {
      var options = app
          .ApplicationServices
          .GetRequiredService<IOptionsMonitor<AppOptions>>();
      return app.UseStaticFiles(new StaticFileOptions()
      {
        FileProvider = new SettingsFileProvider(options)
      });
    }
  }
}
