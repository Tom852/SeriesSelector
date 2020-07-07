using SeriesSelector.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SeriesSelector
{
    [Serializable]
    public class Series : INotifyPropertyChanged
    {
        public string FilesystemPath { get; set; }

        private int _currentIndex;

        public int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
            set
            {
                _currentIndex = value;
                OnPropertyChanged(nameof(CurrentIndex));
                OnPropertyChanged(nameof(CurrentEpisodeAsString));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string CurrentEpisodeAsString
        {
            get
            {
                string filename = GetFileNameOfCurrentEpisode();
                Regex r = new Regex(@"(.* - )(\d{2}x\d{2}.*)");
                Match m = r.Match(filename);
                if (m.Success)
                {
                    return m.Groups[2].Value;
                }
                else
                {
                    return filename;
                }
            }
        }

        public string SeriesNameAsString
        {
            get
            {
                string[] splitted = FilesystemPath.Split('/', '\\');
                LinkedList<string> makeMyLifeEasy = new LinkedList<string>(splitted);
                return makeMyLifeEasy.Last.Value;
            }
        }

        public override string ToString()
        {
            return SeriesNameAsString;
        }

        public Series(string filepath)
        {
            FilesystemPath = filepath;
            CurrentIndex = 0;
        }

        public Series()
        {
        }

        public string[] GetFileList()
        {
            return Directory.GetFiles(FilesystemPath);
        }

        public string GetFullFilePathOfCurrentEpisode()
        {
            try
            {
                var allFiles = GetFileList();
                return allFiles[CurrentIndex];
            }
            catch (IndexOutOfRangeException)
            {
                if (GetFileList().Any())
                {
                    CurrentIndex = 0;
                    return GetFileList()[CurrentIndex];
                }
                else
                {
                    return "Error - Folder Empty - Please Remove";
                }
            }
            catch (DirectoryNotFoundException)
            {
                return "Error - Folder not Found - Please Remove";
            }
        }

        public string GetFileNameOfCurrentEpisode()
        {
            var full = GetFullFilePathOfCurrentEpisode();
            var justName = Path.GetFileName(full);
            return justName;
        }

        public void Play()
        {
            System.Diagnostics.Process.Start(GetFullFilePathOfCurrentEpisode());
            Increase();
        }

        public void OpenInExplorer()
        {
            var path = GetFullFilePathOfCurrentEpisode();
            string argument = $"/e, /select, \"{path}\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        public void Increase(int amount = 1)
        {
            if (CanIncrease(amount))
            {
                CurrentIndex += amount;
            }
        }

        public void Decrease(int amount = 1)
        {
            if (CanDecrease(amount))
            {
                CurrentIndex -= amount;
            }
        }

        public bool CanIncrease(int amount = 1)
        {
            var maxIndex = GetFileList().Length;
            return CurrentIndex + amount < maxIndex;
        }

        public bool CanDecrease(int amount)
        {
            return CurrentIndex - amount >= 0;
        }
    }
}