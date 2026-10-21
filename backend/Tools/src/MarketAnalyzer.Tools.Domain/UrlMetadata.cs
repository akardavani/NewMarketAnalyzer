using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using MarketAnalyzer.Tools.Domain;

namespace MarketAnalyzer.Tools.Application
{
    public class UrlMetadata { public string Url { get; init; } = ""; public string Title { get; init; } = ""; public string? ImageUrl { get; init; } public BookmarkKind Kind { get; init; } }

    /// <summary>دریافت عنوان/تصویر صفحه با محافظت در برابر SSRF (بررسی IP در لحظهٔ اتصال، بدون redirect خودکار)</summary>
    public static class UrlMetadataFetcher
    {
        public static BookmarkKind GuessKind(Uri u)
        {
            var p = u.AbsolutePath.ToLowerInvariant();
            if (p.EndsWith(".pdf")) return BookmarkKind.Pdf;
            if (Regex.IsMatch(p, @"\.(png|jpe?g|gif|webp|svg|bmp)$")) return BookmarkKind.Image;
            if (Regex.IsMatch(p, @"\.(mp4|webm|mkv|mov)$") || u.Host.EndsWith("youtube.com") || u.Host == "youtu.be") return BookmarkKind.Video;
            if (Regex.IsMatch(p, @"\.(docx?|xlsx?|pptx?|txt|csv)$")) return BookmarkKind.Document;
            return BookmarkKind.Link;
        }

        public static bool IsPublic(IPAddress ip)
        {
            if (ip.IsIPv4MappedToIPv6) ip = ip.MapToIPv4();
            if (IPAddress.IsLoopback(ip) || ip.Equals(IPAddress.Any) || ip.Equals(IPAddress.IPv6Any)) return false;
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                return !(ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || ip.IsIPv6Multicast || (ip.GetAddressBytes()[0] & 0xFE) == 0xFC);
            var b = ip.GetAddressBytes();
            return !(b[0] == 10 || b[0] == 127 || b[0] == 0 || b[0] >= 224
                || (b[0] == 172 && b[1] >= 16 && b[1] <= 31) || (b[0] == 192 && b[1] == 168)
                || (b[0] == 169 && b[1] == 254) || (b[0] == 100 && b[1] >= 64 && b[1] <= 127));
        }

        private static readonly HttpClient Client = new(new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            ConnectTimeout = TimeSpan.FromSeconds(4),
            ConnectCallback = async (ctx, ct) =>
            {
                var ips = await Dns.GetHostAddressesAsync(ctx.DnsEndPoint.Host, ct);
                var ip = ips.FirstOrDefault(IsPublic) ?? throw new HttpRequestException("آدرس مجاز نیست.");
                if (ips.Any(i => !IsPublic(i))) throw new HttpRequestException("آدرس مجاز نیست.");
                var s = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
                try { await s.ConnectAsync(new IPEndPoint(ip, ctx.DnsEndPoint.Port), ct); return new NetworkStream(s, true); }
                catch { s.Dispose(); throw; }
            }
        }) { Timeout = TimeSpan.FromSeconds(8) };

        public static async Task<UrlMetadata?> FetchAsync(string url, CancellationToken ct)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https")) return null;
            var kind = GuessKind(uri);
            var fallback = new UrlMetadata { Url = url, Title = uri.Host, Kind = kind, ImageUrl = kind == BookmarkKind.Image ? url : null };
            if (kind != BookmarkKind.Link && kind != BookmarkKind.Video) return fallback;
            try
            {
                for (var hop = 0; hop < 4; hop++)
                {
                    if (IPAddress.TryParse(uri.Host, out var lit) && !IsPublic(lit)) return fallback;
                    using var req = new HttpRequestMessage(HttpMethod.Get, uri);
                    req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; LinkPreviewBot/1.0)");
                    using var res = await Client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                    if ((int)res.StatusCode is >= 300 and < 400 && res.Headers.Location is { } loc)
                    {
                        uri = loc.IsAbsoluteUri ? loc : new Uri(uri, loc);
                        if (uri.Scheme != "http" && uri.Scheme != "https") return fallback;
                        continue;
                    }
                    if (!res.IsSuccessStatusCode) return fallback;
                    if (res.Content.Headers.ContentType?.MediaType?.Contains("html") != true) return fallback;
                    await using var st = await res.Content.ReadAsStreamAsync(ct);
                    var buf = new byte[512 * 1024]; var n = 0;
                    while (n < buf.Length) { var r = await st.ReadAsync(buf.AsMemory(n), ct); if (r == 0) break; n += r; }
                    var html = System.Text.Encoding.UTF8.GetString(buf, 0, n);
                    var title = Meta(html, "og:title") ?? Regex.Match(html, @"<title[^>]*>(.*?)</title>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value;
                    var img = Meta(html, "og:image") ?? Meta(html, "twitter:image");
                    if (img != null && Uri.TryCreate(uri, img, out var iu) && (iu.Scheme == "http" || iu.Scheme == "https")) img = iu.ToString(); else img = null;
                    title = WebUtility.HtmlDecode(title ?? "").Trim();
                    return new UrlMetadata { Url = url, Title = title.Length == 0 ? uri.Host : (title.Length > 300 ? title[..300] : title), ImageUrl = img, Kind = kind };
                }
            }
            catch { }
            return fallback;
        }

        private static string? Meta(string html, string key)
        {
            foreach (Match m in Regex.Matches(html, @"<meta\s[^>]*>", RegexOptions.IgnoreCase))
            {
                var tag = m.Value;
                if (!Regex.IsMatch(tag, $@"(property|name)\s*=\s*[""']{Regex.Escape(key)}[""']", RegexOptions.IgnoreCase)) continue;
                var c = Regex.Match(tag, @"content\s*=\s*""([^""]*)""|content\s*=\s*'([^']*)'", RegexOptions.IgnoreCase);
                var v = c.Groups[1].Success ? c.Groups[1].Value : c.Groups[2].Value;
                if (v.Length > 0) return WebUtility.HtmlDecode(v);
            }
            return null;
        }
    }
}
