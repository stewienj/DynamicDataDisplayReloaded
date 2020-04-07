using System;

namespace DynamicDataDisplay.BitmapGraphs
{
	public class GroupedMarkersCalculationProgressArgs : EventArgs
	{
		public double? Progress { get; set; }

		public TimeSpan CalculationTime { get; set; }
	}
}
