using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DynamicDataDisplay.Common.Auxiliary.DataSearch
{
	public class NearestPointSearch2D
	{

		private BlockingCollection<PositionAndCheckDistance> _mousePoints = new BlockingCollection<PositionAndCheckDistance>();

		public NearestPointSearch2D()
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
			// There's going to be some threading issues with dynamic data, so this shouldn't be used on dynamic data
			var lineGraphs = _children?.OfType<LineGraph>().ToArray();

			if (lineGraphs == null)
			{
				return;
			}
			Point position = currentPos.DataPosition;
			Point bottomLeft = currentPos.DataBottomLeft;
			Point topRight = currentPos.DataTopRight;
			LineGraph closestXLeftGraph = null;
			LineGraph closestXRightGraph = null;
			LineGraph closestYAboveGraph = null;
			LineGraph closestYBelowGraph = null;
			Point? closestXLeftPoint = null;
			Point? closestXRightPoint = null;
			Point? closestYAbovePoint = null;
			Point? closestYBelowPoint = null;

			ConcurrentDictionary<LineGraph, List<Point>> _filteredPointsByGraph = new ConcurrentDictionary<LineGraph, List<Point>>();

			lineGraphs.AsParallel().ForAll((lineGraph) =>
			{
				IEnumerable<Point> allPoints = null;
				_dispatcher.Invoke(() =>
		  {
			  allPoints = lineGraph.GetPoints();
		  });
				var filteredPoints = allPoints.Where(p => InRange(p, bottomLeft, topRight)).ToList();
				_filteredPointsByGraph[lineGraph] = filteredPoints;
			});

			foreach (var lineGraph in lineGraphs)
			{
				var filteredPoints = _filteredPointsByGraph[lineGraph];

				if (filteredPoints.Count > 0)
				{
					foreach (var point in filteredPoints)
					{
						if (!closestXLeftPoint.HasValue || point.X > closestXLeftPoint.Value.X && point.X <= position.X)
						{
							closestXLeftGraph = lineGraph;
							closestXLeftPoint = point;
						}
						if (!closestXRightPoint.HasValue || point.X < closestXRightPoint.Value.X && point.X >= position.X)
						{
							closestXRightGraph = lineGraph;
							closestXRightPoint = point;
						}
						if (!closestYBelowPoint.HasValue || point.Y > closestYBelowPoint.Value.Y && point.Y <= position.Y)
						{
							closestYBelowGraph = lineGraph;
							closestYBelowPoint = point;
						}
						if (!closestYAbovePoint.HasValue || point.Y < closestYAbovePoint.Value.Y && point.Y >= position.Y)
						{
							closestYAboveGraph = lineGraph;
							closestYAbovePoint = point;
						}
					}
				}
			}

			if (_mousePoints.Count > 0)
			{
				return;
			}

			LineGraph closestGraph = null;
			Point? closestPoint = null;
			double screenDistance = 0;
			var closestPoints = new[] { closestXLeftPoint, closestXRightPoint, closestYAbovePoint, closestYBelowPoint };
			var closestGraphs = new[] { closestXLeftGraph, closestXRightGraph, closestYAboveGraph, closestYBelowGraph };
			var lastMousePos = _lastMousePosInData.DataToScreen(currentPos.Transform);
			for (int i = 0; i < closestPoints.Length; ++i)
			{
				if (closestPoints[i] != null)
				{
					double screenDistanceTest = (closestPoints[i].Value.DataToScreen(currentPos.Transform) - lastMousePos).Length;
					if (screenDistanceTest < currentPos.ScreenDistance.X)
					{
						if (screenDistanceTest < screenDistance || !closestPoint.HasValue)
						{
							screenDistance = screenDistanceTest;
							closestPoint = closestPoints[i];
							closestGraph = closestGraphs[i];
						}
					}

				}
			}
			if (closestPoint.HasValue)
			{
				lock (_updateLock)
				{
					_lastLockPos = new PositionAndCheckDistance(closestPoint.Value.DataToScreen(currentPos.Transform), currentPos.ScreenDistance, currentPos.Transform);
				}
				OnNearestPointUpdated(closestPoint.Value, true, closestGraph.Description.ToString());
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

		private Dispatcher _dispatcher = null;
		private ReadOnlyObservableCollection<DependencyObject> _children = null;
		public ReadOnlyObservableCollection<DependencyObject> ContentBoundsHosts
		{
			get
			{
				return _children;
			}
			set
			{
				_dispatcher = Dispatcher.CurrentDispatcher;
				_children = value;
			}
		}

		public event EventHandler<NearestPointSearchArgs> NearestPointUpdated;
	}
}
