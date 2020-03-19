using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Windows;

namespace PaletteControlSampleApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		HSBPalette palette = new HSBPalette();
		public Window1()
		{
			InitializeComponent();
			paletteControl.Palette = palette;
		}

		private void startSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			palette.Start = startSlider.Value;
		}

		private void widthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			palette.Width = widthSlider.Value;
		}
	}
}
