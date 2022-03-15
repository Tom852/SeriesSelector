using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesSelector
{
    public class IndexedEpisode
    {
        public string OriginalFileName { get; set; }
        public bool HasIndexes { get; set; }
        public int SeasonIndex { get; set; }
        public int EpisodeIndex { get; set; }
        public bool HasEpisodeName { get; set; }
        public string EpisodeName { get; set; }
    }
}
