using LegendSample.Pages;
using System.Windows;
using System.Windows.Controls;

namespace LegendSample
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

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			//AddPage(new LineChartInLegendPage());
			//AddPage(new DisconnectedLegendMode());
			AddPage(new AnotherLookOfLegendItemPage());

			tabControl.SelectedIndex = 0;
		}

		private void AddPage(Page page)
		{
			TabItem item = new TabItem { Content = new Frame { Content = page }, Header = page.Title };
			tabControl.Items.Add(item);
		}
	}
}
