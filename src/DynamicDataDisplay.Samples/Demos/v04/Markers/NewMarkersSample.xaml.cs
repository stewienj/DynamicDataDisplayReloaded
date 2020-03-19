using NewMarkersSample.Pages;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.v04.Markers
{
	/// <summary>
	/// Interaction logic for NewMarkersSample.xaml
	/// </summary>
	public partial class NewMarkersSample : Page
	{
		public NewMarkersSample()
		{
			InitializeComponent();
		}

		public void AddPage(Page page)
		{
			TabItem tab = new TabItem { Header = page.Title };
			tab.Content = new Frame { Content = page };
			tabControl.Items.Add(tab);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AddPage(new BarChartPage());
			AddPage(new PieChartPage());
			AddPage(new AcceptableRangePage());
			AddPage(new BigPointArrayPage());
			AddPage(new ColumnChartSample());
			AddPage(new DifferentBuildInMarkersPage());
			AddPage(new StockMarkersPage());
		}
	}
}
