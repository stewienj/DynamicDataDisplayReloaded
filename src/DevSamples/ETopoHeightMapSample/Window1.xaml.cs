#define new

using DynamicDataDisplay;
using DynamicDataDisplay.Maps.Charts.TiledRendering;
using System.IO;
using System.Windows;

namespace ETopoHeightMapSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			if (File.Exists("etopo2.dos.bin"))
			{
#if new
				var heightMap = new ETopoHeightMapGraph();
				Viewport2D.SetContentBounds(heightMap, new DataRect(-180, -85, 360, 170));

				OneThreadRenderingMap map = new OneThreadRenderingMap(heightMap) { DrawDebugBounds = true };
				plotter.Children.Add(map);
#else
				plotter.Children.Add(new ETopoHeightMapGraph());
#endif
			}
			else
			{
				MessageBox.Show("etopo2.dos.bin file is not found.\n" +
					"Please download it from http://www.ngdc.noaa.gov/mgg/global/relief/ETOPO2/ETOPO2-2001/\n" +
					"and place in the same folder with this sample.",
					"Data file is not found!");
			}
		}
	}
}
