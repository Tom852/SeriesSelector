using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SeriesSelector
{
    public class SeriesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Series> SeriesList { get; set; } = new ObservableCollection<Series>();

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}