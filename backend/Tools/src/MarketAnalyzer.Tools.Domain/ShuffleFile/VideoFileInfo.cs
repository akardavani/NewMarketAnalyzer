using System.Collections.Generic;

namespace MarketAnalyzer.Tools.Domain
{
    public class VideoFileInfo
    {
        public int RootFolderUsed { get; set; } = 0;

        public string RootFolderPath { get; set; } = string.Empty;
        public string RootFolderName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public string UniqueFileName => RootFolderName + FileName;

        // مطابق کد قدیمی FileInformation، واحد مدت «دقیقه» است.
        public long Duration { get; set; }

        public bool IsUsed { get; set; } = false;

        public void AddItem(VideoFileInfo item)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));

            RootFolderPath = item.RootFolderPath;
            RootFolderName = item.RootFolderName;
            FilePath = item.FilePath;
            FileName = item.FileName;
            Duration = item.Duration;
        }
    }
}

