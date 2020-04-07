using DynamicDataDisplay;
using System.Windows;
using System.Windows.Media;

namespace PlotterLayoutPanels
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void ChartPlotter_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.LeftPanel.Background = Brushes.DodgerBlue.MakeTransparent(0.4);
			plotter.RightPanel.Background = Brushes.Salmon.MakeTransparent(0.4);
			plotter.BottomPanel.Background = Brushes.PaleGreen;
			plotter.TopPanel.Background = Brushes.Gold;

			plotter.HeaderPanel.Background = Brushes.GreenYellow;
			plotter.FooterPanel.Background = Brushes.DarkOrchid.MakeTransparent(0.4);

			//NumericAxis horizAxis = plotter.MainHorizontalAxis as NumericAxis;
			//horizAxis.AxisControl.IsStaticAxis = true;

			//NumericAxis vertAxis = plotter.MainVerticalAxis as NumericAxis;
			//vertAxis.AxisControl.IsStaticAxis = true;
		}
	}
}
