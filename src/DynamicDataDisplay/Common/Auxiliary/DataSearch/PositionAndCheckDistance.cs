using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary.DataSearch
{
	public class PositionAndCheckDistance
	{
		public PositionAndCheckDistance(Point screenPosition, CoordinateTransform transform) : this(screenPosition, new Vector(), transform)
		{

		}

		public PositionAndCheckDistance(Point screenPosition, Vector screenDistance, CoordinateTransform transform)
		{
			ScreenPosition = screenPosition;
			ScreenDistance = screenDistance;

			DataPosition = screenPosition.ScreenToData(transform);
			DataTopRight = (screenPosition + screenDistance).ScreenToData(transform);
			DataBottomLeft = (screenPosition - screenDistance).ScreenToData(transform);
			Transform = transform;
		}

		public Point DataPosition { get; private set; }
		public Point DataTopRight { get; private set; }
		public Point DataBottomLeft { get; private set; }
		public CoordinateTransform Transform { get; private set; }
		public Point ScreenPosition { get; private set; }
		public Vector ScreenDistance { get; private set; }
	}
}
