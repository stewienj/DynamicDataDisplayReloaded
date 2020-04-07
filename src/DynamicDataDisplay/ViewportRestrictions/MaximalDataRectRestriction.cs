using System;
using System.Windows;

namespace DynamicDataDisplay.ViewportRestrictions
{
	public class MaximalDataRectRestriction : MaximalRectRestriction
	{

		public MaximalDataRectRestriction(DataRect dataRect) : base(dataRect)
		{
		}

		public override DataRect Apply(DataRect oldDataRect, DataRect newDataRect, Viewport2D viewport)
		{
			var storedMaxRect = internalMaxRect;
			internalMaxRect = internalMaxRect.DataToViewport(viewport.Transform.DataTransform);

			if (internalMaxRect.Width.IsNaN() || internalMaxRect.Height.IsNaN())
			{
				return newDataRect;
			}

			if (internalMaxRect.XMin.IsNaN())
			{
				internalMaxRect.XMin = double.NegativeInfinity;
			}
			if (internalMaxRect.YMin.IsNaN())
			{
				internalMaxRect.YMin = double.NegativeInfinity;
			}
			if (internalMaxRect.XMax.IsNaN())
			{
				internalMaxRect.XMax = double.PositiveInfinity;
			}
			if (internalMaxRect.YMax.IsNaN())
			{
				internalMaxRect.YMax = double.PositiveInfinity;
			}

			var retVal = base.Apply(oldDataRect, newDataRect, viewport);
			internalMaxRect = storedMaxRect;
			return retVal;
		}

		private Point Merge(Point point1, Point point2, Func<double, double, double> merger)
		{
			return new Point(merger(point1.X, point2.X), merger(point1.Y, point2.Y));
		}
	}
}
