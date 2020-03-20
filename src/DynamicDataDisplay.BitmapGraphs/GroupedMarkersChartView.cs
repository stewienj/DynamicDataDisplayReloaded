using System.Windows;
using System.Windows.Media.Imaging;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
	public class GroupedMarkersChartView : PointsBitmapGraphBase
	{
		private GroupedMarkers _groupedMarkers = new GroupedMarkers();

		public GroupedMarkersChartView() : base()
		{
			var aggregatedBlockPixelWidth = 40;
			var aggregatedBlockPixelHeight = 25;
			//40, 25

			_pointsCalculator = new GroupedMarkersAggregatorAndSelector(this) { AggregatedBlockPixelWidth = aggregatedBlockPixelWidth, AggregatedBlockPixelHeight = aggregatedBlockPixelHeight };
			_pointsCalculator.NewPointsReady += PointsCalculator_NewPointsReady;
			_pointsCalculator.SelectedPointsChanged += PointsCalculator_SelectedPointsChanged;

		}


		protected override BitmapSource RenderDataFrame(DataRect data, Rect output, RenderRequest renderRequest)
		{
			var points = GetLastPointsInDataRect(data);
			if (points == null)
			{
				return EmptyBitmap;
			}
			else
			{
				_groupedMarkers.Transform = Plotter2D.Transform;
				_groupedMarkers.LastSelection = LastSelectionArgs;
				return _groupedMarkers.DrawImage((int)output.Width, (int)output.Height, points, renderRequest);
			}
		}
	}
}
