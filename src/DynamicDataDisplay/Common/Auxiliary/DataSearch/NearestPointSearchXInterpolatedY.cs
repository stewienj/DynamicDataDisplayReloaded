using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DynamicDataDisplay.Common.Auxiliary.DataSearch
{
	/// <summary>
	/// Keeps the cursor in the current X position, finds there nearest values either side of the cursor in the X plane only,
	/// then interpolates the position so the cursor can be positioned on the Y plot
	/// </summary>
	public class NearestPointSearchXInterpolatedY : NearestPointSearchInterpolatedPerpendicular
	{

		protected override double? GetInterpolatedValue(List<Point> filteredPoints, Point positionInData)
		{
			var lower = filteredPoints.Where(p => p.X <= positionInData.X).MaxX();
			var upper = filteredPoints.Where(p => p.X >= positionInData.X).MinX();

			lower = lower ?? upper;
			upper = upper ?? lower;

			if (lower.HasValue && upper.HasValue)
			{
				if (lower.Value.X >= upper.Value.X)
				{
					return lower.Value.Y;
				}
				else
				{
					double diffX = upper.Value.X - lower.Value.X;
					double partDiffX = positionInData.X - lower.Value.X;
					double diffY = upper.Value.Y - lower.Value.Y;
					double yValue = lower.Value.Y + (partDiffX * diffY) / diffX;
					return yValue;
				}
			}
			return null;
		}

	}
}
