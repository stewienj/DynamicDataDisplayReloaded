using DynamicDataDisplay;
using DynamicDataDisplay.Common.Auxiliary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace DynamicDataDisplay.Markers.Filters
{
	public sealed class UnitingPointGroupFilter : GroupFilter
	{
		protected internal override IEnumerable<Point> Filter(IEnumerable<Point> series)
		{
			var visible = Visible;
			var screenRect = Output;
			var transform = Transform;
			var markerSize = MarkerSize;

			Stack<Point> rootPoints = new Stack<Point>();

			using (new DisposableTimer("filter"))
			{
				foreach (Point point in series/*.OrderBy(p => p.X).ThenBy(p => p.Y)*/)
				{
					var pointOnScreen = point.DataToScreen(transform);

					double minDistance = double.PositiveInfinity;

					foreach (var root in rootPoints)
					{
						var rootInScreen = root.DataToScreen(transform);
						var distance = (rootInScreen - pointOnScreen).Length;
						if (distance < markerSize && distance < minDistance)
						{
							minDistance = distance;
							break;
						}
					}

					if (minDistance > markerSize)
					{
						rootPoints.Push(point);
					}
				}
			}

			Debug.WriteLine("Non-parallel points: " + rootPoints.Count);

			return rootPoints;
		}
	}
}
