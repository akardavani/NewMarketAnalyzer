using Microsoft.Extensions.Options;
using System;
using System.Drawing.Text;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

namespace MarketAnalyzer.Captcha.Internal
{
  internal class CaptchaFontProvider
  {
    private readonly ImageCaptchaOptions _options;
    private readonly PrivateFontCollection _privateFontCollection = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public CaptchaFontProvider(IOptions<ImageCaptchaOptions> options)
    {
      if (options is null)
      {
        throw new ArgumentNullException(nameof(options));
      }

      _options = options.Value;

      foreach (var font in _options.EmbeddedFonts)
      {
        AddFont(font());
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="familyName"></param>
    /// <param name="emSize"></param>
    /// <param name="style"></param>
    /// <returns></returns>
    public Font GetFont(string familyName, float emSize, FontStyle style)
    {
      var fontFamily = _privateFontCollection
          .Families
          .FirstOrDefault(x => x.Name == familyName);
      if (fontFamily is not null)
      {
        return new Font(fontFamily, emSize, style);
      }

      return new Font(familyName, emSize, style);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fullFileName"></param>
    public void AddFont(string fullFileName)
    {
      AddFont(File.ReadAllBytes(fullFileName));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fontBytes"></param>
    public void AddFont(byte[] fontBytes)
    {
      var handle = GCHandle.Alloc(fontBytes, GCHandleType.Pinned);
      IntPtr pointer = handle.AddrOfPinnedObject();
      try
      {
        _privateFontCollection.AddMemoryFont(pointer, fontBytes.Length);
      }
      finally
      {
        handle.Free();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
      _privateFontCollection.Dispose();
    }
  }
}
