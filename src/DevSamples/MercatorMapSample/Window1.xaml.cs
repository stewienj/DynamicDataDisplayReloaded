using Microsoft.Research.DynamicDataDisplay;
using System.Windows;

namespace MercatorMapSample
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

		private void SetVisibleBtn_Click(object sender, RoutedEventArgs e)
		{
			plotter.Visible = new DataRect(-0.1, -0.1, 0.01, 0.01);
		}

		private void JumpLeft_Click(object sender, RoutedEventArgs e)
		{
			plotter.Visible = DataRect.Offset(plotter.Visible, -plotter.Visible.Width, 0);
		}
	}
}
