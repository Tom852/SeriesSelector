using SeriesSelector.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

// should rethink exception handling
namespace SeriesSelector
{
    [Serializable]
    public class Series : INotifyPropertyChanged
    {
        private SeriesIndexer indexer = new SeriesIndexer();

        public event PropertyChangedEventHandler PropertyChanged;


        public string FolderPath { get; set; }

        private string _currentFileName;
        public string CurrentFileName
        {
            get
            {
                return _currentFileName;
            }
            set
            {
                _currentFileName = value;
                OnPropertyChanged(nameof(EpisodeDisplayName));
            }
        }




        public Series(string folderPath)
        {
            FolderPath = folderPath;
            CurrentFileName = this.GetFileList().FirstOrDefault();
        }

        public Series() { } // for serializer


        public string SeriesDisplayName
        {
            get
            {
                {
                    return FolderPath?.Split('/', '\\').ToList().Last() ?? "Error: Folder not set";
                }
            }
        }


        public string EpisodeDisplayName
        {
            get
            {
                try
                {
                    var (hasError, message) = Validate();
                    if (hasError)
                    {
                        return message;
                    }

                    if (!Properties.Settings.Default.IsSeriesMode)
                    {
                        return this.CurrentFileName;
                    }

                    var data = this.indexer.GetSeasonAndIndex(this.CurrentFileName);
                    if (!data.success)
                    {
                        return this.CurrentFileName;
                    }

                    var seasonTwoDigit = data.season.ToString("d2");
                    var episodeTwoDigit = data.episode.ToString("d2");
                    if (data.hasName)
                    {
                        return $"{seasonTwoDigit}x{episodeTwoDigit}: {data.episodeName}";
                    }
                    else
                    {
                        return $"Season {seasonTwoDigit} Episode {episodeTwoDigit}";
                    }
                }
                catch
                {
                    return "Error";
                }

            }
        }


        public override string ToString() => SeriesDisplayName;


        private FileListCache fileListCache = new FileListCache();

        private List<string> GetFileList()
        {
            // whith a little cache i can call this method extensively.
            Func<List<string>> filegetter = () =>
                 Directory.GetFiles(FolderPath)
                .Where(f => !File.GetAttributes(f).HasFlag(FileAttributes.Hidden))
                .Select(f => Path.GetFileName(f))
                .ToList();

            return fileListCache.GetFiles(filegetter);

        }

        public string GetFullFilePathOfCurrentEpisode()
            => Path.Combine(this.FolderPath, this.CurrentFileName);

        public void Play()
        {
            System.Diagnostics.Process.Start(GetFullFilePathOfCurrentEpisode());
            Increase();

        }

        public void OpenInExplorer()
        {
            var argument = "";
            if (File.Exists(GetFullFilePathOfCurrentEpisode()))
            {
                argument = $"/e, /select, \"{GetFullFilePathOfCurrentEpisode()}\"";
            }
            else if (Directory.Exists(FolderPath))
            {
                argument = $"\"{FolderPath}\"";
            }
            System.Diagnostics.Process.Start("explorer.exe", argument);

        }

        public void Increase(int amount = 1)
        {

                if (CanIncrease(amount))
                {
                    var currentIndex = this.GetIndexOfCurrentEpisode();
                    CurrentFileName = this.GetFileList()[currentIndex + amount];
                }

        }

        public void ResetToFirstFile()
        {
            var files = GetFileList();
            this.CurrentFileName = files.First();
        }

        public void Decrease(int amount = 1)
        {
            if (CanDecrease(amount))
            {
                var currentIndex = this.GetIndexOfCurrentEpisode();
                CurrentFileName = this.GetFileList()[currentIndex - amount];
            }
        }

        public bool CanIncrease(int amount = 1)
        {
            var maxIndex = GetFileList().Count;
            return GetIndexOfCurrentEpisode() + amount < maxIndex;
        }

        public bool CanDecrease(int amount)
        {
            return GetIndexOfCurrentEpisode() - amount >= 0;
        }



        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int GetIndexOfCurrentEpisode()
        {
            var index = this.GetFileList().IndexOf(this.CurrentFileName);
            if (index == -1)
            {
                throw new FileNotFoundException("File not found"); // would work otherwise, but unexpecedtily reset your state
            }
            else
            {
                return index;
            }
        }

        private (bool hasError, string errorMessage) Validate()
        {

            if (!Directory.Exists(this.FolderPath))
            {
                return (true, "Folder not found");
            }

            if (string.IsNullOrEmpty(this.CurrentFileName))
            {
                return (true, "Current file data not available");
            }

            if (File.Exists(Path.Combine(this.FolderPath, this.CurrentFileName)))
            {
                return (false, string.Empty);

            }

            if (!GetFileList().Any())
            {
                return (true, "Folder empty");
            }

            if (Properties.Settings.Default.IsSeriesMode)
            {
                var wantedSeriesIndexes = indexer.GetSeasonAndIndex(this.CurrentFileName);
                if (wantedSeriesIndexes.success) {
                    foreach (var file in GetFileList())
                    {
                        var candidateIndexes = indexer.GetSeasonAndIndex(file);
                        if (candidateIndexes.success && candidateIndexes.season == wantedSeriesIndexes.season && candidateIndexes.episode == wantedSeriesIndexes.episode)
                        {
                            this.CurrentFileName = file;
                            return (true, $"File not found. Series Mode reset file to {file}");
                        }
                    }
                }
            }

            return (true, $"File '{this.CurrentFileName}' not found");

        }

        public void Redraw()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EpisodeDisplayName)));
        }
    }
}