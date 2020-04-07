using System;
using System.Windows;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class SizeExtensions
	{
		private const double sizeRatio = 1e-7;
		public static bool EqualsApproximately(this Size size1, Size size2)
		{
			bool widthEquals = Math.Abs(size1.Width - size2.Width) / size1.Width < sizeRatio;
			bool heightEquals = Math.Abs(size1.Height - size2.Height) / size1.Height < sizeRatio;

			return widthEquals && heightEquals;
		}
	}
}
