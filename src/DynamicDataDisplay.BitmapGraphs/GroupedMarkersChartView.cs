using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.BitmapGraphs
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


        public Color MarkerBackground
        {
            get { return (Color)GetValue(MarkerBackgroundProperty); }
            set { SetValue(MarkerBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerBackgroundProperty =
            DependencyProperty.Register("MarkerBackground", typeof(Color), typeof(GroupedMarkersChartView), new PropertyMetadata(Colors.LightGreen, (s,e)=> 
			{ 
				if (s is GroupedMarkersChartView chartView && e.NewValue is Color newColor)
                {
					chartView._groupedMarkers.BackgroundColor = newColor;
                }
			}));



        public Color MarkerBorderColor
        {
            get { return (Color)GetValue(MarkerBorderColorProperty); }
            set { SetValue(MarkerBorderColorProperty, value); }
        }

		// Using a DependencyProperty as the backing store for MarkerBorderColor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MarkerBorderColorProperty =
			DependencyProperty.Register("MarkerBorderColor", typeof(Color), typeof(GroupedMarkersChartView), new PropertyMetadata(Colors.DarkGreen, (s, e) =>
			{
				if (s is GroupedMarkersChartView chartView && e.NewValue is Color newColor)
				{
					chartView._groupedMarkers.BorderColor = newColor;
				}
			}));



        public Color MarkerFontColor
        {
            get { return (Color)GetValue(MarkerFontColorProperty); }
            set { SetValue(MarkerFontColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerFontColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerFontColorProperty =
            DependencyProperty.Register("MarkerFontColor", typeof(Color), typeof(GroupedMarkersChartView), new PropertyMetadata(Colors.Black, (s, e) =>
			{
				if (s is GroupedMarkersChartView chartView && e.NewValue is Color newColor)
				{
					chartView._groupedMarkers.FontColor = newColor;
				}
			}));



	}
}
