using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SeriesSelector.Annotations;

namespace SeriesSelector
{
    public class Series : INotifyPropertyChanged
    {
        private string _filesystemPath;
        private int _currentIndex;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public string FilesystemPath
        {
            get
            {
                return _filesystemPath;
            }
            set
            {
                _filesystemPath = value;
                OnPropertyChanged(nameof(FilesystemPath));
            }

        }

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
            }
        }


        public string[] GetFileList() => Directory.GetFiles(FilesystemPath);

        public string GetFullFilePathOfCurrentEpisode()
        {
            var allFiles = GetFileList();
            var targetFile = allFiles[CurrentIndex];
            return targetFile;
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
            if (CanIncrease())
            {
                CurrentIndex++;
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
