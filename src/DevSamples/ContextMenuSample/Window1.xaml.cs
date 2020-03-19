using Microsoft.Research.DynamicDataDisplay;
using System.Windows;

namespace ContextMenuSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			//plotter.DefaultContextMenu.Remove();
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			rectangle.Fill = ColorHelper.RandomBrush;
		}
	}
}
