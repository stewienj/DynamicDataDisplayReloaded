using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.BitmapGraphs
{
	public class HeatmapChartView : PointsBitmapGraphBase
	{
		private static double DefaultSpread = 8.0;
		private double _spread = DefaultSpread;
		private HeatmapColors _colors = new HeatmapColors();

		public HeatmapChartView() : base(5, 5)
		{
		}

		public string[] ColorScalesAvailable => HeatmapColors.ColorScalesAvailable;


		public string SelectedColorScale
		{
			get { return (string)GetValue(SelectedColorScaleProperty); }
			set { SetValue(SelectedColorScaleProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SelectedColorScale.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedColorScaleProperty =
			DependencyProperty.Register("SelectedColorScale", typeof(string), typeof(HeatmapChartView), new PropertyMetadata("Color Spectrum", (s, e) =>
			{
				if (s is HeatmapChartView control && e.NewValue is string selectedColorScale)
				{
					control._colors.SetColorScale(selectedColorScale);
					control.UpdateVisualization();
				}
			}));

		public double DotSpread
		{
			get { return (double)GetValue(DotSpreadProperty); }
			set { SetValue(DotSpreadProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DotSpread.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DotSpreadProperty =
			DependencyProperty.Register("DotSpread", typeof(double), typeof(HeatmapChartView), new PropertyMetadata(DefaultSpread, (s, e) =>
			{
				if (s is HeatmapChartView control && e.NewValue is double spread)
				{
					control._spread = spread;
					control.UpdateVisualization();
				}
			}));


		protected override BitmapSource RenderDataFrame(DataRect data, Rect output, RenderRequest renderRequest)
		{
			var points = GetLastPointsInDataRect(data);
			if (points == null)
			{
				return EmptyBitmap;
			}
			else
			{
                return Heatmap.Instance.DrawImage((int)output.Width, (int)output.Height, (int)Math.Round(_spread), points.Select(p => (p.Point, p.Count)), _colors.ColorMap, renderRequest);
			}
		}
	}
}
