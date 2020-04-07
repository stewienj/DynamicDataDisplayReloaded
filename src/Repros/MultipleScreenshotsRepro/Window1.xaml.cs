using DynamicDataDisplay;
using DynamicDataDisplay.DataSources;
using System;
using System.Linq;
using System.Windows;

namespace MultipleScreenshotsRepro
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

		Random rnd = new Random();
		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			var xs = Enumerable.Range(0, 100).Select(i => i * 0.01);
			var xds = xs.AsXDataSource();
			double shift = rnd.NextDouble();
			var yds = xs.Select(x => Math.Sin(x * 10 + shift)).AsYDataSource();

			var ds = xds.Join(yds);
			var chart = plotter.AddLineGraph(ds);
			//plotter.LegendVisibility

			chart.Loaded += new RoutedEventHandler(chart_Loaded);
			plotter.SaveScreenshot(@"C:\rightNext.png");
		}

		void chart_Loaded(object sender, RoutedEventArgs e)
		{
			LineGraph graph = (LineGraph)sender;
			graph.Loaded -= chart_Loaded;

			plotter.SaveScreenshot(@"C:\loaded.png");
		}
	}
}
