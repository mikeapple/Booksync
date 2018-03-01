using System;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace BookSync.Extensions
{
    public class ColorFromRgb : IMarkupExtension
    {
		public ColorFromRgb() { }
		public ColorFromRgb(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
		}

		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return Color.FromRgb(R, G, B);
		}

    }
}
