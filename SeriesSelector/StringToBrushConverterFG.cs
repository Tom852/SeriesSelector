using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SeriesSelector
{
    public class StringToBrushConverterFG : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string)value;
            try
            {
                var cs = name.ToRGB();

                int brightness = cs[0] + cs[1] + cs[2];  //remember: byte + byte = int

                if (brightness < 100)
                {
                    return Brushes.White;
                }
                return Brushes.Black;
            }
            catch (IndexOutOfRangeException)
            {
                return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}