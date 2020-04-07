using System.Collections.Generic;
using System.Windows;

namespace DynamicDataDisplay.Charts.NewLine
{
	public sealed class LinePart
	{
		public IEnumerable<Point> Points { get; set; }
		public double Parameter { get; set; }
	}
}
