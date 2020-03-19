using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary.DataSearch
{
	/// <summary>
	/// Keeps the cursor in the current X position, finds there nearest values either side of the cursor in the X plane only,
	/// then interpolates the position so the cursor can be positioned on the Y plot
	/// </summary>
	public class NearestPointSearchYInterpolatedX : NearestPointSearchInterpolatedPerpendicular
	{

		protected override double? GetInterpolatedValue(List<Point> filteredPoints, Point positionInData)
		{
			var lower = filteredPoints.Where(p => p.Y <= positionInData.Y).MaxY();
			var upper = filteredPoints.Where(p => p.Y >= positionInData.Y).MinY();

			lower = lower ?? upper;
			upper = upper ?? lower;

			if (lower.HasValue && upper.HasValue)
			{
				if (lower.Value.Y >= upper.Value.Y)
				{
					return lower.Value.X;
				}
				else
				{
					double diffY = upper.Value.Y - lower.Value.Y;
					double partDiffY = positionInData.Y - lower.Value.Y;
					double diffX = upper.Value.X - lower.Value.X;
					double xValue = lower.Value.X + (partDiffY * diffX) / diffY;
					return xValue;
				}
			}
			return null;
		}

	}
}
