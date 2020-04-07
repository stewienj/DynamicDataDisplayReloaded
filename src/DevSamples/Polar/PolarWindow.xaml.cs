using DynamicDataDisplay;
using DynamicDataDisplay.DataSources;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Polar
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class PolarWindow : Window
	{
		ChartPlotter plotter = new ChartPlotter();
		public PolarWindow()
		{
			InitializeComponent();

			grid.Children.Add(plotter);

			const int N = 100;
			var rs = Enumerable.Range(0, N).Select(i => (double)1);
			var phis = Enumerable.Range(0, N).Select(i => (i * (360.0 / (N - 1)).DegreesToRadians()));

			EnumerableDataSource<double> xs = new EnumerableDataSource<double>(rs);
			xs.SetXMapping(x => x);
			EnumerableDataSource<double> ys = new EnumerableDataSource<double>(phis);
			ys.SetYMapping(y => y);
			CompositeDataSource ds = new CompositeDataSource(xs, ys);

			LineGraph line = new LineGraph();
			line.DataTransform = new CompositeDataTransform(new PolarToRectTransform(), new RotateDataTransform(0.5, new Point(3, 0)));
			line.Stroke = Brushes.Blue;
			line.StrokeThickness = 1;
			line.DataSource = ds;
			plotter.Children.Add(line);
		}
	}
}
