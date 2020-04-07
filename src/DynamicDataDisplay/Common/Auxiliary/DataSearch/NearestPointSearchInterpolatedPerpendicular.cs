using DynamicDataDisplay.Charts.Navigation;
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
	/// <summary>
	/// Keeps the cursor in the current X position, finds there nearest values either side of the cursor in the X plane only,
	/// then interpolates the position so the cursor can be positioned on the Y plot
	/// </summary>
	public abstract class NearestPointSearchInterpolatedPerpendicular
	{
		private Point _lastMousePosInData = new Point(0, 0);
		private double[] _lastYPositionsInData = new double[0];

		private BlockingCollection<Point> _mousePointsInData = new BlockingCollection<Point>();

		public NearestPointSearchInterpolatedPerpendicular()
		{
			DoMousePosProcessing();
		}

		public void UpdatePositionInData(Point mousePos, CoordinateTransform transform)
		{
			Point mousePosInData = mousePos.ScreenToData(transform);
			_mousePointsInData.Add(mousePosInData);
			_lastMousePosInData = mousePosInData;
			OnNearestPointUpdated();
		}

		private void DoMousePosProcessing()
		{
			Task task = new Task(() =>
			{
				foreach (Point pos in _mousePointsInData.GetLastestConsumingEnumerable())
				{
					if (_mousePointsInData.Count == 0)
					{
						CheckPosAgainstData(pos);
						Thread.Sleep(20);
					}
				}
			}, TaskCreationOptions.LongRunning);
			task.Start();
		}

		private void CheckPosAgainstData(Point positionInData)
		{
			ParallelQuery<LineGraph> lineGraphs = null;
			// There's going to be some threading issues with dynamic data, so this shouldn't be used on dynamic data
			_dispatcher.Invoke(() =>
			{
				lineGraphs = _children?.OfType<LineGraph>().Where(lg => CursorFollowYGraph.GetSnapTo(lg) || CursorFollowXGraph.GetSnapTo(lg)).ToArray().AsParallel();
			});

			if (lineGraphs == null)
			{
				return;
			}

			ConcurrentDictionary<LineGraph, List<Point>> _filteredPointsByGraph = new ConcurrentDictionary<LineGraph, List<Point>>();
			ConcurrentDictionary<LineGraph, double> interpolatedCursorPoints = new ConcurrentDictionary<LineGraph, double>();

			lineGraphs.ForAll((lineGraph) =>
			{
				IEnumerable<Point> allPoints = null;
				_dispatcher.Invoke(() =>
		  {
			  allPoints = lineGraph.GetPoints();
		  });
				var filteredPoints = allPoints.ToList();
				_filteredPointsByGraph[lineGraph] = filteredPoints;
			});

			lineGraphs.ForAll((lineGraph) =>
			{
				var filteredPoints = _filteredPointsByGraph[lineGraph];
				var interpoloatedPoint = GetInterpolatedValue(filteredPoints, positionInData);
				if (interpoloatedPoint.HasValue)
				{
					interpolatedCursorPoints[lineGraph] = interpoloatedPoint.Value;
				}
			});

			OnNearestPointUpdated(interpolatedCursorPoints.Values.ToArray());
		}

		protected abstract double? GetInterpolatedValue(List<Point> filteredPoints, Point positionInData);



		object _updateLock = new object();
		private void OnNearestPointUpdated(double[] yPositions = null)
		{
			lock (_updateLock)
			{
				_lastYPositionsInData = yPositions ?? _lastYPositionsInData;
				NearestPointUpdated?.Invoke(this, new NearestPointSearchInterpolatedArgs(_lastMousePosInData, _lastYPositionsInData));
			}
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

		public event EventHandler<NearestPointSearchInterpolatedArgs> NearestPointUpdated;
	}
}
