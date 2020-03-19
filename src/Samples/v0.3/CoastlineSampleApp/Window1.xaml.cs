using System.Windows;

namespace CoastlineSampleApp
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
			plotter.Viewport.Visible = new Rect(-180, -90, 360, 180);
		}
	}
}
