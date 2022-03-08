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
            // todo: auslagern 'get episode and season', bei missing file erkennen und suchen.
            get
            {
                try
                {
                    var (hasError, message) = Validate();
                    if (!string.IsNullOrEmpty(message))
                    {
                        return message;
                    }

                    Regex r = new Regex(@"(.* - )([0-3]?\d[x\-_eE]?(\d{2}).*)");
                    Match m = r.Match(this.CurrentFileName);
                    if (m.Success)
                    {
                        return m.Groups[2].Value;
                    }
                    else
                    {
                        return this.CurrentFileName;
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
            // is cached because i call this method 'too' often and may be slow on slow network devices - leads to bad UX
            // note - did not solve the problem, may be c ached anyway.
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
                return (true, "Error - Folder not Found");
            }

            if (this.CurrentFileName != null && File.Exists(Path.Combine(this.FolderPath, this.CurrentFileName)))
            {
                return (false, string.Empty);

            }

            if (GetFileList().Any())
            {
                var firstFile = GetFileList().First();
                // could just reset to first file.... not sure.
                return (true, $"File '{this.CurrentFileName}' was not found");

            }
            else
            {
                return (true, "Error - Folder empty");

            }
        }
    }
}