using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    public class NewsQueruResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Source { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
    }
}
