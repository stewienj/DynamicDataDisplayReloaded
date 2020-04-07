using DynamicDataDisplay;
using DynamicDataDisplay.DataSources;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DescriptionChangeOnTheFly
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

		LineGraph chart;
		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			double[] xs = new double[] { 1, 2, 3 };
			double[] ys = new double[] { 0.1, 0.4, 0.2 };

			var ds = xs.AsXDataSource().Join(ys.AsYDataSource());

			chart = plotter.AddLineGraph(ds, "Chart1");
		}

		private void changeDescrBtn_Click(object sender, RoutedEventArgs e)
		{
			chart.Stroke = Brushes.Red;
			chart.StrokeThickness = 3;
			chart.Description = new PenDescription("Chart2");
		}
	}
}
