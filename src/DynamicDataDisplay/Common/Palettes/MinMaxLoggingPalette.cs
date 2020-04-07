using System.Diagnostics;
using System.Windows.Media;

namespace DynamicDataDisplay.Common.Palettes
{
	/// <summary>
	/// Represents a palette that calculates minimal and maximal values of interpolation coefficient and every 100000 calls writes these values 
	/// to DEBUG console.
	/// </summary>
	public class MinMaxLoggingPalete : DecoratorPaletteBase
	{
		double min = double.MaxValue;
		double max = double.MinValue;
		int counter = 0;
		public override Color GetColor(double t)
		{
			if (t < min) min = t;
			if (t > max) max = t;
			counter++;

			if (counter % 100000 == 0)
			{
				Debug.WriteLine("Palette: Min = " + min + ", max = " + max);
			}

			return base.GetColor(t);
		}
	}
}
