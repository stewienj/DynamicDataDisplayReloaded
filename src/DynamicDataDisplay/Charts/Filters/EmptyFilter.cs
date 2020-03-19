using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Filters
{
	public sealed class EmptyFilter : PointsFilterBase
	{
		public override List<Point> Filter(List<Point> points)
		{
			return points;
		}
	}
}
