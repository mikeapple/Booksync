using System;
using System.Globalization;
using Xamarin.Forms;

namespace BookSync.Converters
{
    public class TrueToFalseConverter : IValueConverter
    {
        public TrueToFalseConverter()
        {
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : !(bool)value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
