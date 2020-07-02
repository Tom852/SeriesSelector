using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SeriesSelector
{
    public class ProgressToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double) value;
            bool isNotRunning = val <= 1e-10d || val >= 100d;
            return isNotRunning ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}