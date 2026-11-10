using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Tools.Application;

public sealed class ShuffleVideoOptions
{
    public bool Enabled { get; set; }
    public string[] SourceRoots { get; set; } = Array.Empty<string>();
    public string[] OutputRoots { get; set; } = Array.Empty<string>();
    public string StateDirectory { get; set; } = "";
    public string FfprobePath { get; set; } = "ffprobe";
}
public sealed class ShuffleVideoRequest
{
    public string SourcePath { get; set; } = "";
    public string OutputPath { get; set; } = "";
    public string Prefix { get; set; } = "Mix-";
    public int FolderCount { get; set; } = 1;
    public int TargetMinutes { get; set; } = 60;
}
public sealed record ShuffleVideoFile(string Path, string Category, long Bytes, long ModifiedTicks, double Seconds);
public sealed record ShuffleVideoFolder(string Name, double Seconds, bool Complete, List<ShuffleVideoFile> Files);
public sealed record ShuffleVideoPlan(string Id, DateTime ExpiresUtc, string SourcePath, string OutputPath,
    int TargetMinutes, int AvailableFiles, int PreviouslyUsed, long TotalBytes,
    List<ShuffleVideoFolder> Folders, List<string> Warnings);
public sealed record ShuffleVideoResult(string Path, int CopiedFiles, long CopiedBytes);
public sealed class ShuffleVideoConflictException : Exception
{
    public ShuffleVideoConflictException(string message) : base(message) { }
}
