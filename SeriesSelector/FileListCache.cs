using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesSelector
{
    public class FileListCache
    {
        public readonly TimeSpan maxAge = TimeSpan.FromMinutes(1);

        public List<IndexedEpisode> Filerinos { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool WasCreatedInSeriesMode { get; set; }

        private SeriesIndexer indexer = new SeriesIndexer();

        public List<IndexedEpisode> GetFiles(Func<List<string>> getFiles)
        {
            var age = DateTime.Now - CreatedAt;
            var currentMode = Properties.Settings.Default.IsSeriesMode;

            if (age > maxAge || currentMode != WasCreatedInSeriesMode)
            {
                var rawFiles = getFiles();
                if (currentMode)
                {
                    this.Filerinos = rawFiles.Select(raw => indexer.GetSeasonAndIndex(raw))
                        .OrderByDescending(i => i.HasIndexes)
                        .ThenBy(i => i.SeasonIndex)
                        .ThenBy(i => i.EpisodeIndex)
                        .ThenBy(i => i.OriginalFileName)
                        .ToList();
                }
                else
                {
                    this.Filerinos = rawFiles.OrderBy(s => s).Select(r => new IndexedEpisode()
                    {
                        OriginalFileName = r,
                        HasEpisodeName = false,
                        HasIndexes = false,
                    }).ToList();
                }

                CreatedAt = DateTime.Now;
                WasCreatedInSeriesMode = currentMode;
            }

            return this.Filerinos;
        }
    }
}
