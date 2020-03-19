using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	/// <summary>
	/// Represents an editor of points' position of ViewportPolyline or ViewportPolygon.
	/// New version written by John Stewien 10th April 2019. Old version had a race condition.
	/// </summary>
	[ContentProperty("Polyline")]
	public class PolylineEditor : IPlotterElement
	{
		private List<DraggablePoint> _draggablePoints = new List<DraggablePoint>();
		private bool _updatingFromPolyline = false;
		private bool _updatingFromDraggablePoints = false;
		private bool _removePolyLine = true;
		private ViewportPolylineBase _polyline;
		private DispatcherPriority _priority = DispatcherPriority.Normal;

		/// <summary>
		/// Initializes a new instance of the <see cref="PolylineEditor"/> class.
		/// </summary>
		public PolylineEditor()
		{
		}

		/// <summary>
		/// Schedule copying the polyline point positions to the draggable points
		/// </summary>
		/// <param name="addPolyLine"></param>
		private void ScheduleSyncPolylineToDraggables(bool addPolyLine)
		{
			_removePolyLine = addPolyLine;
			if (_updatingFromDraggablePoints)
				return;

			_plotter?.Dispatcher.BeginInvoke((Action)(() => SyncPolylineToDraggables(addPolyLine)), _priority);
		}

		/// <summary>
		/// Sychronizes Draggable points, and the polyline
		/// </summary>
		private void SyncPolylineToDraggables(bool addPolyline)
		{
			if (_updatingFromDraggablePoints || _plotter == null)
				return;

			_updatingFromPolyline = true;

			if (addPolyline)
			{
				_plotter.Children.Add(_polyline);
			}

			var points = _polyline.Points.ToList();

			while (_draggablePoints.Count < points.Count)
			{
				var draggablePoint = new DraggablePoint();
				draggablePoint.Position = points[_draggablePoints.Count];
				_draggablePoints.Add(draggablePoint);
				DependencyPropertyDescriptor.FromProperty(DraggablePoint.PositionProperty, typeof(DraggablePoint)).AddValueChanged(draggablePoint, ScheduleSyncDraggablesToPolyline);

				_plotter?.Children.Add(draggablePoint);
			}

			while (_draggablePoints.Count > points.Count)
			{
				var draggablePoint = _draggablePoints[_draggablePoints.Count - 1];
				_draggablePoints.RemoveAt(_draggablePoints.Count - 1);
				_plotter?.Children.Remove(draggablePoint);
			}

			for (int i = 0; i < points.Count; ++i)
			{
				var draggablePoint = _draggablePoints[i];
				draggablePoint.Position = points[i];
				draggablePoint.Visibility = _polyline.Visibility;
			}

			_updatingFromPolyline = false;
		}

		/// <summary>
		/// Schedule propagating the new draggable positions to the polyline
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScheduleSyncDraggablesToPolyline(object sender, EventArgs e)
		{
			if (_updatingFromPolyline)
				return;

			_plotter?.Dispatcher.BeginInvoke((Action)(() => SyncDraggablesToPolyline()), _priority);
		}

		private void SyncDraggablesToPolyline()
		{
			if (_updatingFromPolyline || _plotter == null)
				return;

			_updatingFromDraggablePoints = true;
			var points = _draggablePoints.Select(ds => ds.Position);
			_polyline.Points = new System.Windows.Media.PointCollection(points);
			_updatingFromDraggablePoints = false;
		}

		/// <summary>
		/// Gets or sets the polyline, to edit points of which.
		/// </summary>
		/// <value>The polyline.</value>
		[NotNull]
		public ViewportPolylineBase Polyline
		{
			get { return _polyline; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Polyline");

				if (_polyline != value)
				{
					_polyline = value;

					var visibilityProperty = DependencyPropertyDescriptor.FromProperty(ViewportPolylineBase.VisibilityProperty, typeof(ViewportPolylineBase));
					visibilityProperty.AddValueChanged(value, (s, e) => ScheduleSyncPolylineToDraggables(false));

					var pointsProperty = DependencyPropertyDescriptor.FromProperty(ViewportPolylineBase.PointsProperty, typeof(ViewportPolylineBase));
					pointsProperty.AddValueChanged(_polyline, (s, e) => ScheduleSyncPolylineToDraggables(false));

					if (_plotter != null)
					{
						ScheduleSyncPolylineToDraggables(true);
					}
				}
			}
		}


		/// <summary>
		/// Removes all the objects the editor added to the plotter.
		/// </summary>
		/// <param name="plotter"></param>

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			_plotter = (Plotter2D)plotter;

			if (_polyline != null)
			{
				ScheduleSyncPolylineToDraggables(!_plotter.Children.Contains(_polyline));
			}
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			var polyline = _polyline;
			_plotter = null;
			_polyline = null;

			// This needs to be pushed to the next frame as we
			// are in the middle of handling a collection changed event
			// from the plotter children, so removing anything from the
			// plotter here will throw an exception
			plotter?.Dispatcher?.BeginInvoke((Action)(() =>
			{
				if (polyline != null)
				{
			  // RemoveLineFromPlotter(plotter);
			  foreach (var draggablePoint in _draggablePoints)
					{
						plotter?.Children.Remove(draggablePoint);
					}

					if (_removePolyLine)
					{
						plotter?.Children.Remove(polyline);
					}

					_draggablePoints.Clear();
				}
			}), _priority);
		}

		private Plotter2D _plotter;
		/// <summary>
		/// Gets the parent plotter of chart.
		/// Should be equal to null if item is not connected to any plotter.
		/// </summary>
		/// <value>The plotter.</value>
		public Plotter Plotter
		{
			get { return _plotter; }
		}

		#endregion
	}
}
