using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Tools.Application;

public sealed class DuplicateScanRequest
{
    public List<string> Paths { get; set; } = new();
    public bool Recursive { get; set; } = true;
}
public sealed record DuplicateGroup(string Hash, long Size, List<string> Paths);
public sealed record DuplicateScanResult(int ScannedFiles, int HashedFiles, int SkippedEntries,
    long DuplicateBytes, List<DuplicateGroup> Groups, List<string> Warnings);

/// <summary>Read-only, bounded scan. Register as singleton to limit concurrent disk scans.</summary>
public sealed class DuplicateFileScanner
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private static readonly StringComparison Comparison = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    private static readonly StringComparer Comparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

    public async Task<DuplicateScanResult> ScanAsync(DuplicateScanRequest request,
        string[] allowedRoots, CancellationToken ct)
    {
        if (!await _gate.WaitAsync(0, ct))
            throw new InvalidOperationException("یک جست‌وجوی دیگر در حال اجراست؛ کمی بعد تلاش کنید.");
        try
        {
            return await Task.Run(() => ScanCoreAsync(request, allowedRoots, ct), ct);
        }
        finally { _gate.Release(); }
    }

    private static string Normalize(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
            throw new ArgumentException("مسیرها باید کامل و مطلق باشند.");
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
    }

    // Reject links in every ancestor too, not just links discovered during traversal.
    private static void EnsureNoLinks(string path)
    {
        for (var item = new DirectoryInfo(path); item != null; item = item.Parent)
            if ((item.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new ArgumentException("مسیرهای شامل لینک یا junction مجاز نیستند.");
    }

    private static async Task<DuplicateScanResult> ScanCoreAsync(DuplicateScanRequest request,
        string[] allowedRoots, CancellationToken ct)
    {
        if (request.Paths == null || request.Paths.Count is < 1 or > 20)
            throw new ArgumentException("بین ۱ تا ۲۰ مسیر وارد کنید.");
        var roots = allowedRoots.Select(Normalize).ToArray();
        var paths = request.Paths.Select(Normalize).Distinct(Comparer).ToArray();
        foreach (var path in paths)
        {
            if (!roots.Any(root => path.Equals(root, Comparison) ||
                path.StartsWith(Path.EndsInDirectorySeparator(root) ? root : root + Path.DirectorySeparatorChar, Comparison)))
                throw new ArgumentException("یکی از مسیرها خارج از پوشه‌های مجاز سرور است.");
            if (!Directory.Exists(path)) throw new ArgumentException("یکی از پوشه‌ها وجود ندارد یا قابل دسترسی نیست.");
            EnsureNoLinks(path);
        }

        var visited = new HashSet<string>(Comparer);
        var files = new Dictionary<string, (long Size, DateTime Modified)>(Comparer);
        var warnings = new List<string>();
        int skipped = 0, hashed = 0;
        void Warn(string path)
        {
            skipped++;
            if (warnings.Count < 100) warnings.Add("رد شد (لینک، عدم دسترسی یا تغییر حین بررسی): " + path);
        }
        var pending = new Stack<string>(paths);
        int entries = 0;
        while (pending.Count > 0)
        {
            ct.ThrowIfCancellationRequested();
            var directory = pending.Pop();
            if (!visited.Add(directory)) continue;
            try
            {
                EnsureNoLinks(directory);
                foreach (var entry in new DirectoryInfo(directory).EnumerateFileSystemInfos())
                {
                    ct.ThrowIfCancellationRequested();
                    if (++entries > 100000) throw new InvalidOperationException("حداکثر ۱۰۰٬۰۰۰ ورودی قابل بررسی است؛ مسیر کوچک‌تری انتخاب کنید.");
                    try
                    {
                        if ((entry.Attributes & FileAttributes.ReparsePoint) != 0) { Warn(entry.FullName); continue; }
                        if (entry is DirectoryInfo)
                        {
                            if (request.Recursive) pending.Push(entry.FullName);
                        }
                        else if (entry is FileInfo file)
                            files.TryAdd(file.FullName, (file.Length, file.LastWriteTimeUtc));
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { Warn(entry.FullName); }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException) { Warn(directory); }
        }
        var groups = new List<DuplicateGroup>();
        foreach (var sizeGroup in files.GroupBy(x => x.Value.Size).Where(g => g.Count() > 1))
        {
            var hashes = new Dictionary<string, List<string>>();
            foreach (var file in sizeGroup)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    EnsureNoLinks(Path.GetDirectoryName(file.Key)!);
                    var info = new FileInfo(file.Key);
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0) { Warn(file.Key); continue; }
                    await using var stream = new FileStream(file.Key, FileMode.Open, FileAccess.Read, FileShare.Read,
                        81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    if (stream.Length != file.Value.Size || info.LastWriteTimeUtc != file.Value.Modified) { Warn(file.Key); continue; }
                    var hash = Convert.ToHexString(await SHA256.HashDataAsync(stream, ct));
                    info.Refresh();
                    if (info.Length != file.Value.Size || info.LastWriteTimeUtc != file.Value.Modified) { Warn(file.Key); continue; }
                    hashed++;
                    if (!hashes.TryGetValue(hash, out var matches)) hashes[hash] = matches = new List<string>();
                    matches.Add(file.Key);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException) { Warn(file.Key); }
            }
            groups.AddRange(hashes.Where(x => x.Value.Count > 1)
                .Select(x => new DuplicateGroup(x.Key, sizeGroup.Key, x.Value.OrderBy(p => p, Comparer).ToList())));
        }
        groups = groups.OrderByDescending(x => x.Size * (x.Paths.Count - 1)).ToList();
        return new DuplicateScanResult(files.Count, hashed, skipped,
            groups.Sum(x => x.Size * (x.Paths.Count - 1)), groups, warnings);
    }
}
