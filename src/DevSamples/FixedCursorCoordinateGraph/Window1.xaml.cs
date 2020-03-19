using System.Windows;

namespace FixedCursorCoordinateGraph
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

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (xSlider != null && ySlider != null)
				cursorGraph.Position = new Point(xSlider.Value, ySlider.Value);
		}
	}
}
