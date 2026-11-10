using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Tools.Domain
{
    public class FolderInfo
    {
        public long Duration { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public bool IsComplete { get; set; } = false;

        public List<VideoFileInfo> FilesInfo { get; set; }
            = new List<VideoFileInfo>();
    }
}
