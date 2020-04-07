using System.Windows.Media;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class ColorExtensions
	{
		public static Color MakeTransparent(this Color color, int alpha)
		{
			color.A = (byte)alpha;
			return color;
		}

		public static Color MakeTransparent(this Color color, double opacity)
		{
			return MakeTransparent(color, (int)(255 * opacity));
		}
	}
}
