using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SeriesSelector.Annotations;

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
