using System.Windows;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class SizeHelper
	{
		public static Size CreateInfiniteSize()
		{
			return new Size(double.PositiveInfinity, double.PositiveInfinity);
		}
	}
}
