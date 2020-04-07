using System.Windows.Media;

namespace DynamicDataDisplay.Common.Palettes
{
	public static class UniformLinearPalettes
	{
		static UniformLinearPalettes()
		{
			blackAndWhitePalette.IncreaseBrightness = false;
			rgbPalette.IncreaseBrightness = false;
			blueOrangePalette.IncreaseBrightness = false;
		}

		private static readonly UniformLinearPalette blackAndWhitePalette =
			new UniformLinearPalette(Colors.Black, Colors.White);

		public static UniformLinearPalette BlackAndWhitePalette
		{
			get { return blackAndWhitePalette; }
		}

		private static readonly UniformLinearPalette rgbPalette =
			new UniformLinearPalette(Colors.Blue, Color.FromRgb(0, 255, 0), Colors.Red);

		public static UniformLinearPalette RedGreenBluePalette
		{
			get { return rgbPalette; }
		}

		private static readonly UniformLinearPalette blueOrangePalette = new UniformLinearPalette(
			Colors.Blue,
			Colors.Cyan,
			Colors.Yellow,
			Colors.Orange);

		public static UniformLinearPalette BlueOrangePalette
		{
			get { return blueOrangePalette; }
		}
	}
}
