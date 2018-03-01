using System;
using System.Globalization;
using Xamarin.Forms;
using System.IO;

namespace BookSync.Converters
{
	public class EmptyStringToHeightConverter : IValueConverter
	{
		public EmptyStringToHeightConverter()
		{
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value.ToString()) ? 1 : 25;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
