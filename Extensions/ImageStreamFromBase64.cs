using System;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using System.IO;

namespace BookSync.Extensions
{
	public class ImageStreamFromBase64 : IMarkupExtension
    {
		public string EncodedImage { get; set; }

        public ImageStreamFromBase64()
        {
            
        }

        public ImageStreamFromBase64(string encodedImage)
        {
            EncodedImage = encodedImage;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (string.IsNullOrEmpty(EncodedImage))
				return null;

			byte[] bytes = Convert.FromBase64String(EncodedImage);

            using (MemoryStream ms = new MemoryStream(bytes))
			{
                return ImageSource.FromStream(() => ms);
			}
        }
	}
}
