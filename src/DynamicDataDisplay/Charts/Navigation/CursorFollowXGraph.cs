using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.Common.Auxiliary.DataSearch;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Adds to ChartPlotter two crossed lines, the Y position is bound to mouse cursor,
	/// the X position tracks the data as you move the mouse cursor up and down. The line that
	/// gets followed in the one that has the attached dependency property (xaml):
	/// d3:CursorFollowXGraph.SnapTo="True"
	/// </summary>
	public partial class CursorFollowXGraph : CursorCoordinateGraph
	{
		private NearestPointSearchYInterpolatedX _nearestPointSearch = new NearestPointSearchYInterpolatedX();

		/// <summary>
		/// Initializes a new instance of the <see cref="CursorFollowYGraph"/> class.
		/// </summary>
		public CursorFollowXGraph()
		{
			_nearestPointSearch.NearestPointUpdated += _nearestPointSearch_NearestPointUpdated;

		}

		private void _nearestPointSearch_NearestPointUpdated(object sender, NearestPointSearchInterpolatedArgs e)
		{
			if (e.MousePosInData.IsFinite())
			{
				Task.Run(() =>
				{
					Dispatcher.Invoke(() =>
			{
				if (e.PositionsInData.Count() > 0)
				{
					UpdateUIRepresentation(e.MousePosInData, new Point(e.PositionsInData.Last(), e.MousePosInData.Y));
				}
				else
				{
					UpdateUIRepresentation(e.MousePosInData, e.MousePosInData);
				}
			});
				});
			}
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
			DependencyProperty.RegisterAttached("SnapTo", typeof(bool), typeof(CursorFollowXGraph), new PropertyMetadata(false));


	}
}
