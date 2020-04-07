using DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class PlacementExtensions
	{
		public static bool IsBottomOrTop(this AxisPlacement placement)
		{
			return placement == AxisPlacement.Bottom || placement == AxisPlacement.Top;
		}
	}
}
