using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.v03
{
	/// <summary>
	/// Interaction logic for Maps.xaml
	/// </summary>
	public partial class Maps : Page
	{
		public Maps()
		{
			InitializeComponent();

			((NumericAxis)plotter.MainHorizontalAxis).LabelProvider.LabelStringFormat = "{0}°";
			((NumericAxis)plotter.MainVerticalAxis).LabelProvider.LabelStringFormat = "{0}°";

			UIElement messageGrid = (UIElement)Resources["warningMessage"];
			plotter.MainCanvas.Children.Add(messageGrid);
		}
	}
}
