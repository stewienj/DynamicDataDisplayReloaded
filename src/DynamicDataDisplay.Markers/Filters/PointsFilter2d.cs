using DynamicDataDisplay.Markers.DataSources;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace DynamicDataDisplay.Charts.NewLine.Filters
{
	public abstract class PointsFilter2d : DependencyObject, IDisposable
	{
		private IDataSourceEnvironment environment;
		protected internal IDataSourceEnvironment Environment
		{
			get { return environment; }
			set
			{
				environment = value;

				environment.Plotter.Dispatcher.Invoke(() =>
				{
					Viewport = environment.Plotter.Viewport;
				}, DispatcherPriority.Send);
			}
		}

		private Viewport2D viewport;
		protected Viewport2D Viewport
		{
			get { return viewport; }
			set
			{
				viewport = value;
				viewport.PropertyChanged += viewport_PropertyChanged;

				viewport.Dispatcher.Invoke(() =>
				{
					Visible = viewport.Visible;
					Output = viewport.Output;
					Transform = viewport.Transform;
				}, DispatcherPriority.Send);
			}
		}

		private void viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			OnViewportPropertyChanged(e);
		}

		protected virtual void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e) { }

		protected CoordinateTransform Transform { get; private set; }

		protected DataRect Visible { get; private set; }

		protected Rect Output { get; private set; }

		protected static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PointsFilter2d filter = (PointsFilter2d)d;
			filter.RaiseChanged();
		}

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}

		protected internal event EventHandler Changed;

		protected internal abstract IEnumerable<Point> Filter(IEnumerable<Point> series);

		#region IDisposable Members

		public void Dispose()
		{
			viewport.PropertyChanged -= viewport_PropertyChanged;
		}

		#endregion
	}
}
