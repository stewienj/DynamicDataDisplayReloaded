﻿using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary.DataSearch;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Adds to ChartPlotter two crossed lines, the X position is bound to mouse cursor,
	/// the Y position tracks the data as you move the mouse cursor up and down. The line that
	/// gets followed in the one that has the attached dependency property (xaml):
	/// d3:CursorFollowYGraph.SnapTo="True"
	/// </summary>
	public partial class CursorFollowYGraph : CursorCoordinateGraph
	{
		private NearestPointSearchXInterpolatedY _nearestPointSearch = new NearestPointSearchXInterpolatedY();

		/// <summary>
		/// Initializes a new instance of the <see cref="CursorFollowYGraph"/> class.
		/// </summary>
		public CursorFollowYGraph()
		{
			_nearestPointSearch.NearestPointUpdated += _nearestPointSearch_NearestPointUpdated;

		}

		private void _nearestPointSearch_NearestPointUpdated(object sender, NearestPointSearchInterpolatedArgs e)
		{
			Task.Run(() =>
			{
				Dispatcher.Invoke(() =>
		  {
				  if (e.PositionsInData.Count() > 0)
				  {
					  UpdateUIRepresentation(e.MousePosInData, new Point(e.MousePosInData.X, e.PositionsInData.Last()));
				  }
				  else
				  {
					  UpdateUIRepresentation(e.MousePosInData, e.MousePosInData);
				  }
			  });
			});
		}

		#region Plotter

		protected override void OnPlotterAttached()
		{
			_nearestPointSearch.ContentBoundsHosts = Plotter2D.Viewport.ContentBoundsHosts;
			base.OnPlotterAttached();
		}

		protected override void OnPlotterDetaching()
		{
			_nearestPointSearch.ContentBoundsHosts = null;
			base.OnPlotterDetaching();
		}

		#endregion

		protected override void UpdateUIRepresentation(Point mousePos, CoordinateTransform transform)
		{
			_nearestPointSearch.UpdatePositionInData(mousePos, transform);
		}

		public static bool GetSnapTo(DependencyObject obj)
		{
			return (bool)obj.GetValue(SnapToProperty);
		}

		public static void SetSnapTo(DependencyObject obj, bool value)
		{
			obj.SetValue(SnapToProperty, value);
		}

		// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SnapToProperty =
			DependencyProperty.RegisterAttached("SnapTo", typeof(bool), typeof(CursorFollowYGraph), new PropertyMetadata(false));


	}
}
