using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace MarketAnalyzer.Captcha
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
  public class ImageCaptchaOptions
  {
    /// <summary>
    /// The default cookie prefix, which is ".AspNetCore.ImageCaptcha.".
    /// </summary>
    public static readonly string DefaultCookiePrefix = ".AspNetCore.ImageCaptcha";

    private CookieBuilder _cookieBuilder = new()
    {
      HttpOnly = true,

      // Check the comment on CookieBuilder for more details
      IsEssential = true,

      SameSite = SameSiteMode.Strict,

      // Some browsers do not allow non-secure endpoints to set cookies with a 'secure' flag or overwrite cookies
      // whose 'secure' flag is set (http://httpwg.org/http-extensions/draft-ietf-httpbis-cookie-alone.html).
      // Since mixing secure and non-secure endpoints is a common scenario in applications, we are relaxing the
      // restriction on secure policy on some cookies by setting to 'None'. Cookies related to authentication or
      // authorization use a stronger policy than 'None'.
      SecurePolicy = CookieSecurePolicy.None
    };

    /// <summary>
    /// 
    /// </summary>
    public CookieBuilder Cookie
    {
      get => _cookieBuilder;
      set => _cookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the height of the generated captcha image. (defaults to 180px);
    /// </summary>
    public int Width { get; set; } = 180;

    /// <summary>
    /// Gets or sets the height of the generated captcha image. (defaults to 48px);
    /// </summary>
    public int Height { get; set; } = 48;

    /// <summary>
    /// Gets or sets the padding of the generated captcha image. (defaults to 5px);
    /// </summary>
    public Padding Padding { get; set; } = new Padding(5);

    /// <summary>
    /// Gets or sets the length of the generated captcha code.
    /// </summary>
    public int CodeLength { get; set; } = 6;

    /// <summary>
    /// 
    /// </summary>
    public int MaxDuplicateCharacters { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public Color ForeColor { get; set; } = Color.White;

    /// <summary>
    /// 
    /// </summary>
    public Color BackColor { get; set; } = Color.White;

    /// <summary>
    /// 
    /// </summary>
    public Color TextColor { get; set; } = Color.Gray;

    /// <summary>
    /// 
    /// </summary>
    public PathString RequestPath { get; set; } = new PathString("/captcha");

    /// <summary>
    /// 
    /// </summary>
    public bool SuppressXFrameOptionsHeader { get; set; }

    /// <summary>
    /// Gets or sets restrict the use the captcha to the requesting IP.
    /// </summary>
    public bool RestrictToIP { get; set; } = true;

    /// <summary>
    /// Gets or sets the cookie lifetime. (defaults to 5 minutes).
    /// </summary>
    public TimeSpan LifeTime { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the name of the font family used to draw the captcha image. (defaults to 'Times New Roman').
    /// </summary>
    public string FontName { get; set; } = "Times New Roman";

    /// <summary>
    /// 
    /// </summary>
    public List<Func<byte[]>> EmbeddedFonts { get; } = new List<Func<byte[]>>();


    /// <summary>
    /// Gets or sets the font style used to draw the captcha image.
    /// </summary>
    public FontStyle FontStyle { get; set; } = FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout;

    /// <summary>
    /// Gets or sets the characters used to generate the captcha code.
    /// (defaults to 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz').
    /// </summary>
    public string Characters { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
  }
}
