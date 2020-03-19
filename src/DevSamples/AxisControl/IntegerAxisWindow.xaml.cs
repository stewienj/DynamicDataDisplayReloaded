using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Windows;

namespace AxisControlSample
{
	/// <summary>
	/// Interaction logic for IntegerAxisWindow.xaml
	/// </summary>
	public partial class IntegerAxisWindow : Window
	{
		public IntegerAxisWindow()
		{
			InitializeComponent();

			VerticalIntegerAxis verticalAxis = new VerticalIntegerAxis();
			var labels = new string[] { "One", "Two", "Three", "Four", "Five", "Six" };
			verticalAxis.LabelProvider = new CollectionLabelProvider<string>(labels);
			((IntegerTicksProvider)verticalAxis.TicksProvider).MaxStep = 1;

			plotter.MainVerticalAxis = verticalAxis;
		}
	}
}
