using Microsoft.Extensions.Options;
using System;
using System.Drawing.Text;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace MarketAnalyzer.Captcha.Internal
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
  internal class ImageCaptcha
  {
    private readonly ImageCaptchaOptions _options;

    private readonly Random _rnd = new();

    private readonly HatchStyle[] _hatchStyles = Enum.GetValues(typeof(HatchStyle)).Cast<HatchStyle>().ToArray();

    private readonly CaptchaFontProvider _fontProvider;
    private readonly CaptchaTokenProvider _captchaTokenProvider;

    public ImageCaptcha(IOptions<ImageCaptchaOptions> options, CaptchaTokenProvider captchaTokenProvider, CaptchaFontProvider fontProvider)
    {
      if (options is null)
      {
        throw new ArgumentNullException(nameof(options));
      }

      _options = options.Value;
      _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
      _captchaTokenProvider = captchaTokenProvider ?? throw new ArgumentNullException(nameof(captchaTokenProvider));
    }

    public byte[] GenerateAsync(HttpContext httpContext)
    {
      using var bitmap = new Bitmap(_options.Width, _options.Height, PixelFormat.Format24bppRgb);
      using var graphics = Graphics.FromImage(bitmap);
      graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
      graphics.SmoothingMode = SmoothingMode.HighQuality;
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

      var captchaCode = _captchaTokenProvider.CreateAndStoreToken(httpContext).CaptchaCode;

      DrawBackgroundColor(graphics);
      DrawText(graphics, captchaCode);
      DrawInterferenceLines(graphics);
      DrawInterferenceCircles(graphics);
      DrawNoisePoints(graphics);

      using var memoryStream = new MemoryStream();
      bitmap.Save(memoryStream, ImageFormat.Png);
      return memoryStream.ToArray();
    }

    private HatchStyle GetHatchStyle()
    {
      return _hatchStyles[_rnd.Next(_hatchStyles.Length)];
    }

    private void DrawBackgroundColor(Graphics graphics)
    {
      using var hatchBrush = new HatchBrush(GetHatchStyle(), _options.ForeColor, _options.BackColor);
      graphics.FillRectangle(hatchBrush, 0, 0, _options.Width, _options.Height);
    }

    private void DrawText(Graphics graphics, string captchaCode)
    {
      var drawWidth = _options.Width - _options.Padding.Left - _options.Padding.Right;
      var drawHeight = _options.Height - _options.Padding.Top - _options.Padding.Bottom;
      var minFontSize = drawWidth / 5;
      var maxFontSize = drawWidth / 3;
      var charWidth = drawWidth / captchaCode.Length;
      var previousAngle = 0;
      var textBrush = new SolidBrush(_options.TextColor);

      // Draw captcha text char by char
      for (int i = 0; i < captchaCode.Length; i++)
      {
        var x = i * charWidth;
        float fontSize = _rnd.Next(minFontSize, maxFontSize);
        using var font = _fontProvider.GetFont(_options.FontName, fontSize, _options.FontStyle);
        using var stringFormat = new StringFormat()
        {
          Alignment = StringAlignment.Center,
          LineAlignment = StringAlignment.Center
        };
        var rect = new RectangleF(x, _options.Padding.Top, charWidth, drawHeight);

        // Convert the text into a path
        using var graphicsPath = new GraphicsPath();
        graphicsPath.AddString(captchaCode.Substring(i, 1), font.FontFamily, (int)font.Style, font.Size, rect, stringFormat);

        var x1 = x + _rnd.Next(charWidth) / 4F;
        var y1 = _rnd.Next(drawHeight) / 4F;
        var x2 = x + charWidth - _rnd.Next(charWidth) / 4F;
        var y2 = drawHeight - _rnd.Next(drawHeight) / 4F;

        var points = new[]
        {
                    new PointF(x1, y1),
                    new PointF(x2, y1),
                    new PointF(x1, y2),
                    new PointF(x2, y2)
                };

        var matrix = new Matrix();
        graphicsPath.Warp(points, rect, matrix, WarpMode.Perspective, 0);

        // Rotate a bit randomly
        var dx = x + charWidth / 2F;
        var dy = _options.Padding.Top + drawHeight / 2F;
        graphics.TranslateTransform(-dx, -dy, MatrixOrder.Append);

        var angle = previousAngle;
        do
        {
          angle = _rnd.Next(-30, 30);
        }
        while (Math.Abs(angle - previousAngle) < 20);
        previousAngle = angle;

        graphics.RotateTransform(angle, MatrixOrder.Append);
        graphics.TranslateTransform(dx, dy, MatrixOrder.Append);

        // Draw the text.
        graphics.FillPath(textBrush, graphicsPath);
        graphics.ResetTransform();
      }
    }

    private void DrawInterferenceLines(Graphics graphics)
    {

    }

    private void DrawInterferenceCircles(Graphics graphics)
    {

    }

    private void DrawNoisePoints(Graphics graphics)
    {

    }
  }
}
