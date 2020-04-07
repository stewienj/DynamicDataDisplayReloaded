using System.Windows.Media;

namespace DynamicDataDisplay.Common.Palettes
{
	public class TransparentLimitsPalette : DecoratorPaletteBase
	{
		public TransparentLimitsPalette()
		{

		}

		public TransparentLimitsPalette(IPalette palette) : base(palette) { }

		public override Color GetColor(double t)
		{
			if (t < 0 || t > 1) return Colors.Transparent;
			return Palette.GetColor(t);
		}
	}
}
