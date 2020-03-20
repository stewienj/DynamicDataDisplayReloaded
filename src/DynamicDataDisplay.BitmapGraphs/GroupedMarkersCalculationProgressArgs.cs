using System;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
	public class GroupedMarkersCalculationProgressArgs : EventArgs
	{
		public double? Progress { get; set; }

		public TimeSpan CalculationTime { get; set; }
	}
}
