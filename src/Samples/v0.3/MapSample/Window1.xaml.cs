using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;

namespace MapSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			((NumericAxis)plotter.MainHorizontalAxis).LabelProvider.LabelStringFormat = "{0}°";
			((NumericAxis)plotter.MainVerticalAxis).LabelProvider.LabelStringFormat = "{0}°";

			UIElement messageGrid = (UIElement)Resources["warningMessage"];
			plotter.MainCanvas.Children.Add(messageGrid);
		}
	}
}
