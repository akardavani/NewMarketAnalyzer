using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MarketAnalyzer.SettingsFileProvider
{
  internal class SettingsFileProvider : IFileProvider
  {
    public const string FileRequestPath = "/env.js";

    private readonly Lazy<IFileInfo> _fileInfo;

    public SettingsFileProvider(IOptionsMonitor<AppOptions> options)
    {
      if (options is null)
      {
        throw new ArgumentNullException(nameof(options));
      }

      _fileInfo = new Lazy<IFileInfo>(() => new SettingsFileInfo(FileRequestPath[1..], options), true);
    }

    public IDirectoryContents GetDirectoryContents(string subpath) => NotFoundDirectoryContents.Singleton;

    public IFileInfo GetFileInfo(string subpath)
    {
      if (subpath == FileRequestPath)
      {
        return _fileInfo.Value;
      }

      return new NotFoundFileInfo(subpath);
    }

    public IChangeToken Watch(string filter) => NullChangeToken.Singleton;

    private class SettingsFileInfo : IFileInfo
    {
      private static readonly object _syncLock = new();
      private byte[] _buffer;

      public SettingsFileInfo(string name, IOptionsMonitor<AppOptions> options)
      {
        Name = name;
        options.OnChange(BuildFileContent);
        BuildFileContent(options.CurrentValue);
      }

      public bool Exists => true;

      public long Length => _buffer.Length;

      public string PhysicalPath => null;

      public string Name { get; }

      public DateTimeOffset LastModified { get; private set; } = DateTimeOffset.Now;

      public bool IsDirectory => false;

      public Stream CreateReadStream() => new MemoryStream(_buffer);

      private void BuildFileContent(AppOptions options)
      {
        lock (_syncLock)
        {
          LastModified = DateTimeOffset.Now;
          var data = JsonSerializer.Serialize(new
          {
            captchaUrl = options.CaptchaUrl,
            captchaLifetime = 300000,
            apiUrl = options.ApiUrl,
            version = "1.0.0"
          });

          data = $"window.appSettings = {data}";

          _buffer = Encoding.UTF8.GetBytes(data);
        }
      }
    }
  }
}
