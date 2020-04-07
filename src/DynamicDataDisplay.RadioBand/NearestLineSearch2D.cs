using DynamicDataDisplay;
using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.Common.Auxiliary.DataSearch;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DynamicDataDisplay.RadioBand
{
	public class NearestLineSearch2D
	{
		private BlockingCollection<PositionAndCheckDistance> _mousePoints = new BlockingCollection<PositionAndCheckDistance>();

		public NearestLineSearch2D()
		{
			DoMousePosProcessing();
		}

		public void UpdatePositionInData(Point mousePos, Vector screenDistance, CoordinateTransform transform)
		{
			Point mousePosInData = mousePos.ScreenToData(transform);
			_lastMousePosInData = mousePosInData;
			PositionAndCheckDistance pcd = new PositionAndCheckDistance(mousePos, screenDistance, transform);
			OnNearestPointUpdated(pcd);
		}

		private void DoMousePosProcessing()
		{
			Task task = new Task(() =>
			{
				foreach (PositionAndCheckDistance pos in _mousePoints.GetLastestConsumingEnumerable())
				{
					CheckPosAgainstData(pos);
					Thread.Sleep(20);
				}
			}, TaskCreationOptions.LongRunning);
			task.Start();
		}

		public bool InRange(Point point, Point edge1, Point edge2)
		{
			return _mousePoints.Count == 0 ? InRange(point.X, edge1.X, edge2.X) && InRange(point.Y, edge1.Y, edge2.Y) : false;
		}

		public bool InRange(double pos, double edge1, double edge2)
		{
			return edge1 < edge2 ? (pos > edge1 && pos < edge2) : (pos > edge2 && pos < edge1);
		}

		private void CheckPosAgainstData(PositionAndCheckDistance currentPos)
		{
			IEnumerable<(Point start, Point end, string label)> lines = null;
			IEnumerable<(Point start, Point end, string label)> rectangles = null;
			Dispatcher.Invoke(() =>
			{
				lines = NearestVisualSources.OfType<NearestLineSource>().Select(nsl => nsl.ItemsSource).Where(items => items != null).SelectMany(x => x).ToList();
				rectangles = NearestVisualSources.OfType<NearestRectangleSource>().Select(nsl => nsl.ItemsSource).Where(items => items != null).SelectMany(x => x).ToList();
			});

			Point dataPosition = currentPos.DataPosition;
			Point screenPosition = currentPos.ScreenPosition;
			Point dataBottomLeft = currentPos.DataBottomLeft;
			Point dataTopRight = currentPos.DataTopRight;
			double? minDistance = currentPos.ScreenDistance.X;
			string closestLine = null;
			Point? closestScreenPoint = null;


			// First see if mouse is inside one of the rectangles
			foreach (var rectangle in rectangles)
			{
				DataRect dataRect = new DataRect(rectangle.start, rectangle.end);
				if (dataRect.Contains(dataPosition))
				{
					minDistance = 0;
					closestScreenPoint = screenPosition;
					closestLine = rectangle.label;
					break;
				}
			}

			if (!closestScreenPoint.HasValue)
			{
				// Get the rectangle sides
				var rectangleLines = rectangles.Select(rect =>
				{
					return new[]
			{
			(rect.start, new Point(rect.start.X, rect.end.Y), rect.label),
			(new Point(rect.start.X, rect.end.Y), rect.end, rect.label),
			(rect.end, new Point(rect.end.X, rect.start.Y), rect.label),
			(new Point(rect.end.X, rect.start.Y), rect.start, rect.label)
				  };
				}).SelectMany(x => x);

				lines = rectangleLines.Concat(lines);

				// Get closest point from the lines
				foreach (var line in lines)
				{
					var screenPoints = (currentPos.Transform.DataToScreen(line.start), currentPos.Transform.DataToScreen(line.end));
					var distanceAndPoint = MathHelper.DistancePointToSegment(screenPosition, screenPoints);
					if (!minDistance.HasValue || distanceAndPoint.distance < minDistance.Value)
					{
						minDistance = distanceAndPoint.distance;
						closestScreenPoint = distanceAndPoint.nearestPoint;
						closestLine = line.label;
					}
				}
			}

			// Mouse has moved, abort
			if (_mousePoints.Count > 0)
			{
				return;
			}

			if (closestScreenPoint.HasValue)
			{
				lock (_updateLock)
				{
					_lastLockPos = new PositionAndCheckDistance(closestScreenPoint.Value, currentPos.ScreenDistance, currentPos.Transform);
				}
				OnNearestPointUpdated(currentPos.Transform.ScreenToData(closestScreenPoint.Value), true, closestLine);
			}
		}

		Point _lastMousePosInData = new Point();
		PositionAndCheckDistance _lastLockPos = null;
		object _updateLock = new object();

		private void OnNearestPointUpdated(PositionAndCheckDistance pcd)
		{
			lock (_updateLock)
			{
				_mousePoints.Add(pcd);
				if (_lastLockPos == null || ((_lastLockPos.ScreenPosition - pcd.ScreenPosition).Length > _lastLockPos.ScreenDistance.X))
				{
					_lastLockPos = null;
					OnNearestPointUpdated(pcd.DataPosition);
				}
			}
		}

		private void OnNearestPointUpdated(Point nearestPoint, bool hasLock = false, string nearestLine = null)
		{
			NearestPointUpdated?.Invoke(this, new NearestPointSearchArgs(_lastMousePosInData, nearestPoint, hasLock, nearestLine));
		}

		public event EventHandler<NearestPointSearchArgs> NearestPointUpdated;

		public NearestVisualSourceCollection NearestVisualSources { get; set; }
		public Dispatcher Dispatcher { get; internal set; }
	}
}
