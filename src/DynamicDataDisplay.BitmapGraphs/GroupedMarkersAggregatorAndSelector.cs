using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
	public class GroupedMarkersAggregatorAndSelector : PointsAggregatorAndSelector
	{
		public GroupedMarkersAggregatorAndSelector(UIElement positionElement) : base(positionElement)
		{
		}

		/// <summary>
		/// TODO: Work out which grouped marker the user has clicked on
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="clickPoint"></param>
		public override void Mouse_LeftSingleClick(object sender, Point clickPoint)
		{
			base.Mouse_LeftSingleClick(sender, clickPoint);
		}
	}
}
