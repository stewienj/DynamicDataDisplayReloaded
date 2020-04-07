using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.Samples.Demos.v03
{
	/// <summary>
	/// Interaction logic for MercatorShaderMap.xaml
	/// </summary>
	public partial class MercatorShaderMaps : Page
	{
		public MercatorShaderMaps()
		{
			InitializeComponent();

			UIElement messageGrid = (UIElement)Resources["warningMessage"];
			plotter.MainCanvas.Children.Add(messageGrid);
		}
	}
}
