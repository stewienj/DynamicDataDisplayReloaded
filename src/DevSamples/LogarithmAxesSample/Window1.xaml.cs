using DynamicDataDisplay;
using System.Windows;

namespace LogarithmAxesSample
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

		private void plotter_Loaded(object sender, RoutedEventArgs e)
		{
			var genericPlotter = plotter.GetGenericPlotter();
			genericPlotter.DataRect = new GenericRect<double, double>(9, 0, 110, 2);
		}
	}
}
