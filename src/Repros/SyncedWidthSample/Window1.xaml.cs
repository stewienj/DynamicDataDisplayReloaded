using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SyncedWidthSample
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

		private void StackPanel_Loaded(object sender, RoutedEventArgs e)
		{
			plotter2.LeftPanel.SetBinding(Panel.WidthProperty, new Binding("Width") { Source = plotter1.LeftPanel });
		}
	}
}
