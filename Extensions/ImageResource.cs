using System;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace BookSync
{
	[ContentProperty("Source")]
	public class ImageResource : IMarkupExtension
    {
		public string Source { get; set; }
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Source == null)
				return null;
			return ImageSource.FromResource(Source);
		}
	}
}
