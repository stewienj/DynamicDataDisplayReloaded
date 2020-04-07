using DynamicDataDisplay;
using System.Windows;
using System.Windows.Controls;


namespace LegendSample.Pages
{
	/// <summary>
	/// Interaction logic for LineChartInLegendPage.xaml
	/// </summary>
	public partial class LineChartInLegendPage : Page
	{
		public LineChartInLegendPage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.AddLineGraph(DataSourceHelper.CreateSineDataSource());
		}
	}
}
