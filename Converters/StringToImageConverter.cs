using System;
using System.Globalization;
using Xamarin.Forms;
using System.IO;

namespace BookSync.Converters
{
    public class StringToImageConverter : IValueConverter
    {
        public StringToImageConverter()
        {
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string encodedImage = value?.ToString();

            if (string.IsNullOrEmpty(encodedImage))
                encodedImage = "/9j/4AAQSkZJRgABAgAAAQABAAD/7QCcUGhvdG9zaG9wIDMuMAA4QklNBAQAAAAAAIAcAmcAFEtrbFNnM2tISGpYcUlCSGZySWxGHAIoAGJGQk1EMDEwMDBhOTkwMTAwMDBmMTAxMDAwMDNmMDIwMDAwNjAwMjAwMDA4YTAyMDAwMGNlMDIwMDAwMTUwMzAwMDA0ODAzMDAwMDZmMDMwMDAwYTIwMzAwMDAwNjA0MDAwMP/bAEMABgQFBgUEBgYFBgcHBggKEAoKCQkKFA4PDBAXFBgYFxQWFhodJR8aGyMcFhYgLCAjJicpKikZHy0wLSgwJSgpKP/bAEMBBwcHCggKEwoKEygaFhooKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKP/CABEIADIAMgMAIgABEQECEQH/xAAaAAEAAwEBAQAAAAAAAAAAAAAAAQMFAgQG/8QAFQEBAQAAAAAAAAAAAAAAAAAAAAH/xAAVAQEBAAAAAAAAAAAAAAAAAAAAAf/aAAwDAAABEQIRAAAB+4LLLZ0kYa+igF1Mm25mXLo65sAT6NEjqUuLzteCzyA2LK7JQETBiCz/xAAfEAABBAEFAQAAAAAAAAAAAAABAAIDECEREiAyMzH/2gAIAQAAAQUCpsTijC4cWd6m9LizJU3pYwmnVpwCdTYymDaxw1BBFxxbk1obZAKki21H04H4v//EABQRAQAAAAAAAAAAAAAAAAAAAED/2gAIAQIRAT8BB//EABQRAQAAAAAAAAAAAAAAAAAAAED/2gAIAQERAT8BB//EAB0QAAEFAAMBAAAAAAAAAAAAAAEAEBEgIQIxUXH/2gAIAQAABj8CbxZtQ5igc1lppiARC15PSx9U8em4/L//xAAhEAEAAQQBBAMAAAAAAAAAAAABEQAQIDEhQVFhcZGhsf/aAAgBAAABPyG294eaMlGLl7S8MMIKxzNxHz4JQmyhIdaQi6KZltwCoEtThsK8iEUjAhux0fugIMXNgiVG2dR2t9Jjv9W//9oADAMAAAERAhEAABDp/b67xb7gx37zyz7/xAAXEQADAQAAAAAAAAAAAAAAAAAAESAQ/9oACAECEQE/EBWsVf/EABgRAQADAQAAAAAAAAAAAAAAAAEAECAR/9oACAEBEQE/EJ3a0Ov/xAAjEAEAAQMEAgIDAAAAAAAAAAABEQAhMRAgQXFRYYHRkbHw/9oACAEAAAE/ENB5B+a/FMYHwWaxnYQLh/dqT0F7i+w/ogX1EgzKL1OxS4SRrEuJq2mEtITKS7BCEwBNN5MO69mFHLg4dSTs1ozSPkcvL8697qplZC6Z+2lkX8G26XyowV//2Q==";

			byte[] bytes = Convert.FromBase64String(encodedImage);

			return ImageSource.FromStream(() => new MemoryStream(bytes));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
