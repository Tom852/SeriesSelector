using System.ComponentModel;

namespace SeriesSelector
{
    public class SeriesViewModel : INotifyPropertyChanged
    {
        public TrulyObservableCollection<Series> SeriesList { get; set; } = new TrulyObservableCollection<Series>();

        private string _time;

        public string Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
                }
            }
        }

        private double _progress;

        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}