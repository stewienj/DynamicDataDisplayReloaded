using System.Collections.Generic;
using System.Windows;

namespace DynamicDataDisplay.Charts.NewLine
{
	public sealed class LineSplitter
	{
		private readonly double xMissingValue = double.NegativeInfinity;
		private readonly double yMissingValue = double.NegativeInfinity;

		public IEnumerable<LinePart> Split(IEnumerable<Point> points)
		{
			List<Point> list = new List<Point>();
			double parameter = 0;
			bool split = false;
			foreach (var point in points)
			{
				if (point.X == xMissingValue)
				{
					parameter = point.Y;
					split = true;
				}
				else if (point.Y == yMissingValue)
				{
					parameter = point.X;
					split = true;
				}
				else
				{
					list.Add(point);
				}

				if (split)
				{
					split = false;
					yield return new LinePart { Points = list, Parameter = parameter };
					list = new List<Point>();
				}
			}
		}
	}
}
