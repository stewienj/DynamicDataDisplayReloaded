using System.Collections.Generic;
using System.Windows;

namespace DynamicDataDisplay.Charts.Filters
{
	public sealed class EmptyFilter : PointsFilterBase
	{
		public override List<Point> Filter(List<Point> points)
		{
			return points;
		}
	}
}
