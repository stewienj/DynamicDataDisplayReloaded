using System.Windows;

namespace MercatorShaderMapSampleApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			UIElement messageGrid = (UIElement)Resources["warningMessage"];
			plotter.MainCanvas.Children.Add(messageGrid);
		}
	}
}
