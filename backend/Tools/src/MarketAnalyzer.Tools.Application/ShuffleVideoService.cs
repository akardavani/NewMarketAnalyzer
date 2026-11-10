using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Tools.Application;

/// <summary>Server-side, single-instance bounded preview/copy service. Never changes source files.</summary>
public sealed class ShuffleVideoService
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Dictionary<string, (int Owner, ShuffleVideoPlan Plan)> _plans = new();
    private static readonly StringComparer Paths = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
    private static readonly StringComparison Comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase)
        { ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".webm", ".m4v", ".mpeg", ".mpg", ".ts" };
    private const int MaxEntries = 10000;
    private const long MaxCopyBytes = 100L * 1024 * 1024 * 1024;

    private static string Absolute(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Path.IsPathFullyQualified(value))
            throw new ArgumentException("مسیر باید کامل و مطلق باشد.");
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(value));
    }
    private static bool Within(string path, string root) => path.Equals(root, Comparison) ||
        path.StartsWith(Path.EndsInDirectorySeparator(root) ? root : root + Path.DirectorySeparatorChar, Comparison);
    private static void NoLinks(string path)
    {
        FileSystemInfo? entry = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path);
        while (entry != null)
        {
            if ((entry.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new ArgumentException("مسیر شامل لینک یا junction مجاز نیست.");
            entry = entry is FileInfo file ? file.Directory : ((DirectoryInfo)entry).Parent;
        }
    }
    private static string Allowed(string value, string[] roots)
    {
        var path = Absolute(value);
        if (!roots.Select(Absolute).Any(root => Within(path, root)))
            throw new ArgumentException("مسیر خارج از محدودهٔ مجاز سرور است.");
        if (!Directory.Exists(path)) throw new ArgumentException("پوشه وجود ندارد یا قابل دسترسی نیست.");
        NoLinks(path);
        return path;
    }
    private static string State(ShuffleVideoOptions options, string source, string output)
    {
        var state = Absolute(options.StateDirectory);
        if (Within(output, source) || Within(source, output) || Within(state, source) ||
            Within(source, state) || Within(state, output) || Within(output, state))
            throw new ArgumentException("پوشه‌های مبدأ، مقصد و سابقه باید جدا و غیرتو‌در‌تو باشند.");
        // Require administrator-created state directory with private write permissions.
        if (!Directory.Exists(state)) throw new ArgumentException("پوشهٔ سابقه روی سرور ایجاد نشده است.");
        NoLinks(state);
        return state;
    }
    private static FileStream LockState(string state)
    {
        var path = Path.Combine(state, "shuffle.lock");
        if (File.Exists(path)) NoLinks(path);
        try { return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); }
        catch (IOException) { throw new ShuffleVideoConflictException("سابقه در اختیار اجرای دیگری است؛ دوباره تلاش کنید."); }
    }
    private static HashSet<string> ReadHistory(string state)
    {
        var path = Path.Combine(state, "used-files.json");
        if (!File.Exists(path)) return new HashSet<string>(Paths);
        NoLinks(path);
        if (new FileInfo(path).Length > 32 * 1024 * 1024) throw new ShuffleVideoConflictException("سابقه بیش از حد بزرگ است؛ مدیر سرور باید آن را آرشیو کند.");
        // Fail closed on malformed history: never silently reuse everything.
        var data = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(path))
            ?? throw new JsonException("Invalid history");
        return new HashSet<string>(data, Paths);
    }
    private static void WriteHistory(string state, HashSet<string> used)
    {
        var temp = Path.Combine(state, Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(stream, used);
                stream.Flush(true);
            }
            File.Move(temp, Path.Combine(state, "used-files.json"), true);
        }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }
    private void Prune()
    {
        foreach (var key in _plans.Where(x => x.Value.Plan.ExpiresUtc <= DateTime.UtcNow).Select(x => x.Key).ToArray()) _plans.Remove(key);
    }
    public async Task<ShuffleVideoPlan> PreviewAsync(ShuffleVideoRequest request, ShuffleVideoOptions options, int owner, CancellationToken ct)
    {
        if (!await _gate.WaitAsync(0, ct)) throw new ShuffleVideoConflictException("یک عملیات دیگر در حال اجراست.");
        try { return await Task.Run(() => PreviewCore(request, options, owner, ct), ct); }
        finally { _gate.Release(); }
    }
    private async Task<ShuffleVideoPlan> PreviewCore(ShuffleVideoRequest r, ShuffleVideoOptions o, int owner, CancellationToken ct)
    {
        if (r.FolderCount is < 1 or > 50 || r.TargetMinutes is < 1 or > 1440)
            throw new ArgumentException("تعداد پوشه باید ۱ تا ۵۰ و مدت هر پوشه ۱ تا ۱۴۴۰ دقیقه باشد.");
        if (r.Prefix == null || !Regex.IsMatch(r.Prefix, @"^[\p{L}\p{N}_-]{1,40}$"))
            throw new ArgumentException("پیشوند باید ۱ تا ۴۰ حرف، عدد، خط تیره یا زیرخط باشد.");
        var source = Allowed(r.SourcePath, o.SourceRoots);
        var output = Allowed(r.OutputPath, o.OutputRoots);
        var state = State(o, source, output);
        using var stateLock = LockState(state);
        var used = ReadHistory(state);
        var warnings = new List<string>();
        void Warn(string text) { if (warnings.Count < 100) warnings.Add(text); }
        var files = new List<ShuffleVideoFile>();
        int entries = 0, previouslyUsed = 0;
        void Count() { ct.ThrowIfCancellationRequested(); if (++entries > MaxEntries) throw new ArgumentException("حداکثر ۱۰٬۰۰۰ ورودی در هر اسکن مجاز است."); }
        // Like the desktop tool: immediate category folders, not arbitrary recursive traversal.
        foreach (var category in new DirectoryInfo(source).EnumerateDirectories())
        {
            Count();
            if ((category.Attributes & FileAttributes.ReparsePoint) != 0) { Warn("لینک رد شد: " + category.Name); continue; }
            try
            {
                NoLinks(category.FullName);
                foreach (var file in category.EnumerateFiles())
                {
                    Count();
                    if (!Extensions.Contains(file.Extension)) continue;
                    if ((file.Attributes & FileAttributes.ReparsePoint) != 0) { Warn("لینک رد شد: " + file.Name); continue; }
                    if (used.Contains(file.FullName)) { previouslyUsed++; continue; }
                    try
                    {
                        NoLinks(file.FullName);
                        var bytes = file.Length;
                        var modified = file.LastWriteTimeUtc.Ticks;
                        var seconds = await DurationAsync(o.FfprobePath, file.FullName, ct);
                        file.Refresh();
                        if (file.Length != bytes || file.LastWriteTimeUtc.Ticks != modified) { Warn("فایل در حال تغییر رد شد: " + file.Name); continue; }
                        if (seconds == null) { Warn("مدت معتبر یا جریان ویدئو پیدا نشد: " + file.Name); continue; }
                        files.Add(new ShuffleVideoFile(file.FullName, category.Name, bytes, modified, seconds.Value));
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { Warn("فایل قابل خواندن نیست: " + file.Name); }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { Warn("پوشه قابل خواندن نیست: " + category.Name); }
        }
        var pool = files.ToList();
        var folders = new List<ShuffleVideoFolder>();
        for (int i = 1; i <= r.FolderCount; i++)
        {
            var selected = new List<ShuffleVideoFile>();
            var categories = new HashSet<string>(Paths);
            double seconds = 0;
            while (seconds < r.TargetMinutes * 60d)
            {
                ct.ThrowIfCancellationRequested();
                var candidates = pool.Where(x => !categories.Contains(x.Category)).ToList();
                if (candidates.Count == 0) break;
                var file = candidates[Random.Shared.Next(candidates.Count)];
                selected.Add(file); pool.Remove(file); categories.Add(file.Category); seconds += file.Seconds;
            }
            folders.Add(new ShuffleVideoFolder(r.Prefix + i, seconds, seconds >= r.TargetMinutes * 60d, selected));
        }
        long total = folders.SelectMany(x => x.Files).Sum(x => x.Bytes);
        if (total > MaxCopyBytes) throw new ArgumentException("حجم انتخاب‌شده بیش از سقف ۱۰۰ گیگابایت است.");
        if (folders.Any(x => !x.Complete)) Warn("ویدئوی کافی از دسته‌های متفاوت موجود نیست. پوشه‌های ناقص فقط با تأیید صریح قابل کپی هستند.");
        Prune();
        foreach (var key in _plans.Where(x => x.Value.Owner == owner).Select(x => x.Key).ToArray()) _plans.Remove(key);
        if (_plans.Count >= 20) throw new ShuffleVideoConflictException("تعداد پیش‌نمایش‌های فعال زیاد است؛ چند دقیقه بعد تلاش کنید.");
        var plan = new ShuffleVideoPlan(Guid.NewGuid().ToString("N"), DateTime.UtcNow.AddMinutes(15), source, output,
            r.TargetMinutes, files.Count, previouslyUsed, total, folders, warnings);
        _plans.Add(plan.Id, (owner, plan));
        return plan;
    }
    private static async Task<double?> DurationAsync(string executable, string path, CancellationToken ct)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(15));
        using var process = new Process { StartInfo = new ProcessStartInfo(executable)
            { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true } };
        foreach (var arg in new[] { "-v", "error", "-protocol_whitelist", "file", "-select_streams", "v:0", "-show_entries", "stream=codec_type:format=duration", "-of", "json", path })
            process.StartInfo.ArgumentList.Add(arg);
        process.Start();
        // Drain both pipes concurrently to avoid process deadlocks.
        var stdout = process.StandardOutput.ReadToEndAsync(timeout.Token);
        var stderr = process.StandardError.ReadToEndAsync(timeout.Token);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
            var text = await stdout; await stderr;
            if (process.ExitCode != 0) return null;
            using var json = JsonDocument.Parse(text);
            if (!json.RootElement.TryGetProperty("streams", out var streams) || streams.GetArrayLength() == 0 ||
                !json.RootElement.TryGetProperty("format", out var format) || !format.TryGetProperty("duration", out var duration)) return null;
            return double.TryParse(duration.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds)
                && double.IsFinite(seconds) && seconds > 0 ? seconds : null;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return null; }
        catch (JsonException) { return null; }
        finally
        {
            if (!process.HasExited) { process.Kill(true); await process.WaitForExitAsync(CancellationToken.None); }
            // Observe pipe task failures on timeout/cancellation too.
            try { await Task.WhenAll(stdout, stderr); } catch (OperationCanceledException) { }
        }
    }
    public async Task<ShuffleVideoResult> RunAsync(string id, bool allowIncomplete, ShuffleVideoOptions options, int owner, CancellationToken ct)
    {
        if (!await _gate.WaitAsync(0, ct)) throw new ShuffleVideoConflictException("یک عملیات دیگر در حال اجراست.");
        try { return await Task.Run(() => RunCore(id, allowIncomplete, options, owner, ct), ct); }
        finally { _gate.Release(); }
    }
    private async Task<ShuffleVideoResult> RunCore(string id, bool allowIncomplete, ShuffleVideoOptions o, int owner, CancellationToken ct)
    {
        Prune();
        if (string.IsNullOrEmpty(id) || !_plans.TryGetValue(id, out var entry) || entry.Owner != owner)
            throw new ShuffleVideoConflictException("پیش‌نمایش منقضی یا نامعتبر است؛ دوباره پیش‌نمایش بگیرید.");
        var plan = entry.Plan;
        if (!allowIncomplete && plan.Folders.Any(x => !x.Complete)) throw new ArgumentException("پوشهٔ ناقص وجود دارد؛ تأیید جداگانه لازم است.");
        var files = plan.Folders.SelectMany(x => x.Files).ToArray();
        if (files.Length == 0) throw new ArgumentException("هیچ فایلی برای کپی انتخاب نشده است.");
        var source = Allowed(plan.SourcePath, o.SourceRoots);
        var output = Allowed(plan.OutputPath, o.OutputRoots);
        var state = State(o, source, output);
        using var stateLock = LockState(state);
        var used = ReadHistory(state);
        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();
            if (used.Contains(file.Path)) throw new ShuffleVideoConflictException("بعضی فایل‌ها در اجرای دیگری استفاده شده‌اند؛ پیش‌نمایش جدید بگیرید.");
            ValidateFile(file);
        }
        _plans.Remove(id); // A copy request is single-use, including failed/cancelled attempts.
        var batch = "Shuffle-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + id;
        var staging = Path.Combine(output, ".partial-" + id);
        var final = Path.Combine(output, batch);
        if (Directory.Exists(staging) || File.Exists(staging) || Directory.Exists(final) || File.Exists(final))
            throw new ShuffleVideoConflictException("پوشهٔ خروجی موجود است؛ پیش‌نمایش جدید بگیرید.");
        Directory.CreateDirectory(staging);
        bool committed = false;
        try
        {
            foreach (var folder in plan.Folders)
            {
                if (folder.Files.Count == 0) continue;
                var target = Path.Combine(staging, folder.Name);
                Directory.CreateDirectory(target);
                for (int i = 0; i < folder.Files.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var file = folder.Files[i];
                    ValidateFile(file); NoLinks(target);
                    var name = $"{i + 1:D4} - {Path.GetFileName(file.Path)}";
                    await using (var input = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true))
                    await using (var dest = new FileStream(Path.Combine(target, name), FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true))
                    {
                        await input.CopyToAsync(dest, ct);
                        if (dest.Length != file.Bytes) throw new ShuffleVideoConflictException("فایل هنگام کپی تغییر کرد.");
                        await dest.FlushAsync(ct);
                    }
                    ValidateFile(file);
                }
            }
            await File.WriteAllTextAsync(Path.Combine(staging, "manifest.json"), JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }), ct);
            ct.ThrowIfCancellationRequested();
            // Commit boundary: reserve history first. No cancellation between reservation and rename.
            // A crash here may conservatively consume files, but will never silently reuse them.
            foreach (var file in files) used.Add(file.Path);
            WriteHistory(state, used);
            Directory.Move(staging, final);
            committed = true;
            return new ShuffleVideoResult(final, files.Length, plan.TotalBytes);
        }
        finally
        {
            if (!committed && Directory.Exists(staging))
            {
                try { Directory.Delete(staging, true); }
                catch (IOException) { /* Administrator can remove this uniquely named partial directory. */ }
                catch (UnauthorizedAccessException) { }
            }
        }
    }
    private static void ValidateFile(ShuffleVideoFile file)
    {
        NoLinks(file.Path);
        var info = new FileInfo(file.Path);
        if (!info.Exists || info.Length != file.Bytes || info.LastWriteTimeUtc.Ticks != file.ModifiedTicks)
            throw new ShuffleVideoConflictException("یک فایل از زمان پیش‌نمایش تغییر کرده است؛ دوباره پیش‌نمایش بگیرید.");
    }
}
