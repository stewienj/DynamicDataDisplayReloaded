using System;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class RandomExtensions
	{
		public static double NextDouble(this Random rnd, double min, double max)
		{
			return min + (max - min) * rnd.NextDouble();
		}
	}
}
