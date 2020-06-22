using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesSelector
{
    [Serializable]
    public class SeriesViewModel
    {
        public ObservableCollection<Series> SeriesList { get; set; } = new ObservableCollection<Series>();
    }
}
