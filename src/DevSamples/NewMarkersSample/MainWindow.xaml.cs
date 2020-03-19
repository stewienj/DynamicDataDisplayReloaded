using NewMarkersSample.Pages;
using System.Windows;
using System.Windows.Controls;

namespace NewMarkersSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public void AddPage(Page page)
		{
			Frame frame = new Frame { Content = page };
			TabItem tab = new TabItem { Content = frame, Header = page.Title };
			tabControl.Items.Add(tab);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//AddPage(new BigPointArrayPage());
			//AddPage(new BarChartPage());
			AddPage(new BarChartDataTriggerPage());
			//AddPage(new LiveTooltipPage());
			//AddPage(new ColumnChartSample());
			//AddPage(new AcceptableValuePage());
			//AddPage(new PieChartPage());
			//AddPage(new StockMarkersPage());
			//AddPage(new CompositeDSPage());
			//AddPage(new DifferentBuildInMarkersPage());
			//AddPage(new VectorFieldPage());
			//AddPage(new EndlessRandomValuesPage());
			//AddPage(new ConditionalBindingPage());
			//AddPage(new LetterFrequencyPage());
			//AddPage(new PieChartAPIPage());
			//AddPage(new DateTimeRectanglesPage());
			//AddPage(new PointSetPage());
			//AddPage(new RotatedEllipsesPage());
			//AddPage(new RenderingMarkersPage());
			//AddPage(new XmlChartPage());

			tabControl.SelectedIndex = 0;
		}
	}
}
