using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using SeriesSelector.Annotations;

namespace SeriesSelector
{
    public class Series : INotifyPropertyChanged
    {
        private string _filesystemPath;
        private int _currentIndex;


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

        public Brush GetAColorBrush
        {
            get
            {
                try
                {
                    char c1 = SeriesNameAsString.ToLower()[0];
                    char c2 = SeriesNameAsString.ToLower()[1];
                    char c3 = SeriesNameAsString.ToLower()[2];


                    Color c = Color.FromRgb(ScaledByteFromChar(c1), ScaledByteFromChar(c2), ScaledByteFromChar(c3));
                    SolidColorBrush brush = new SolidColorBrush(c);
                    return brush;
                }
                catch (IndexOutOfRangeException)
                {
                    return Brushes.DeepPink;
                }
            }
        }

        private byte ScaledByteFromChar(char x)
        {
            int startFrom0 = x - 'a';
            int rangedFrom0To250 = startFrom0 * 10;
            int rangedFrom5To255 = 5 + rangedFrom0To250;
            return (byte) rangedFrom5To255; //if over / underflow happens cause of spaces or numbers etc it does not matter, is just to get a color per series name.
        }



        public Series(string filepath)
        {
            FilesystemPath = filepath;
            CurrentIndex = 0;
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
