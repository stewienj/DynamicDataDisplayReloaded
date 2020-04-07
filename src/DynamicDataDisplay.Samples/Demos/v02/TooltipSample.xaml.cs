using DynamicDataDisplay.Charts.Navigation;
using DynamicDataDisplay.DataSources;
using DynamicDataDisplay.PointMarkers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicDataDisplay.Samples.Demos.v02
{
	/// <summary>
	/// Interaction logic for TooltipSample.xaml
	/// </summary>
	public partial class TooltipSample : Page
	{
		const int N = 100;
		double[] x = new double[N];
		double[] y = new double[N];
		EnumerableDataSource<double> xDataSource;
		EnumerableDataSource<double> yDataSource;

		LineAndMarker<ElementMarkerPointsGraph> chart;
		IPointDataSource ds;

		public TooltipSample()
		{
			InitializeComponent();

			// Prepare data in arrays
			for (int i = 0; i < N; i++)
			{
				x[i] = i * 0.2;
				y[i] = Math.Cos(x[i]);
			}

			// Add data sources:
			yDataSource = new EnumerableDataSource<double>(y);
			yDataSource.SetYMapping(Y => Y);
			yDataSource.AddMapping(ShapeElementPointMarker.ToolTipTextProperty,
				Y => string.Format("Value is {0}", Y));

			xDataSource = new EnumerableDataSource<double>(x);
			xDataSource.SetXMapping(X => X);


			ds = new CompositeDataSource(xDataSource, yDataSource);
			// adding graph to plotter
			chart = plotter.AddLineGraph(ds,
				new Pen(Brushes.LimeGreen, 3),
				new CircleElementPointMarker
				{
					Size = 10,
					Brush = Brushes.Red,
					Fill = Brushes.Orange
				},
				new PenDescription("Cosine"));

			plotter.Children.Add(new CursorCoordinateGraph());

			// Force evertyhing plotted to be visible
			plotter.FitToView();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (chart.MarkerGraph.DataSource != null)
			{
				chart.MarkerGraph.DataSource = null;
			}
			else
			{
				chart.MarkerGraph.DataSource = ds;
			}
		}
	}
}

