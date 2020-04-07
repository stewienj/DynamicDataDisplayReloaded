using DynamicDataDisplay;
using DynamicDataDisplay.DataSources;
using System;
using System.Windows;

namespace LineChartLegendBinding
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		LineGraph graph;
		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			var ds = new Point[] { new Point(0, 0), new Point(1, 1) }.AsDataSource();
			graph = plotter.AddLineGraph(ds);
		}

		private void colorSelector_SelectedValueChanged(object sender, EventArgs e)
		{
			if (graph == null) return;

			graph.Stroke = colorSelector.SelectedBrush;
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (graph == null) return;

			graph.StrokeThickness = e.NewValue;
		}

		int legendPosition = 0;
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// you can pass any values to LegendRight, Legendleft,..., properties, having in mind that they
			// are wrappers on Canvas.Left, Right, etc, properties.
			// Setting e.g. LegendLeft to NaN clears relevant Canvas's property, enabling you to use
			// Canvas.Right property, as Canvas.Left has a precedence over Canvas.Right.

			// cycles legend position in different 4 corners of plotter.
			legendPosition = (++legendPosition) % 4;
			switch (legendPosition)
			{
				case 0:
					plotter.Legend.LegendRight = 10;
					plotter.Legend.LegendTop = 10;
					plotter.Legend.LegendLeft = double.NaN;
					plotter.Legend.LegendBottom = double.NaN;
					break;
				case 1:
					plotter.Legend.LegendRight = double.NaN;
					plotter.Legend.LegendTop = 10;
					plotter.Legend.LegendLeft = 10;
					plotter.Legend.LegendBottom = double.NaN;
					break;
				case 2:
					plotter.Legend.LegendRight = double.NaN;
					plotter.Legend.LegendTop = double.NaN;
					plotter.Legend.LegendLeft = 10;
					plotter.Legend.LegendBottom = 10;
					break;
				case 3:
					plotter.Legend.LegendRight = 10;
					plotter.Legend.LegendTop = double.NaN;
					plotter.Legend.LegendLeft = double.NaN;
					plotter.Legend.LegendBottom = 10;
					break;
				default:
					break;
			}
		}
	}
}
