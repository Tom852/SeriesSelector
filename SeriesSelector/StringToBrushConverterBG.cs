using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SeriesSelector
{
    public class StringToBrushConverterBG : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string) value;
            try
            {
                var cs = name.ToRGB();

                Color c = Color.FromRgb(cs[0], cs[1], cs[2]);
                SolidColorBrush brush = new SolidColorBrush(c);
                return brush;
            }
            catch (IndexOutOfRangeException)
            {
                return Brushes.DeepPink;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}