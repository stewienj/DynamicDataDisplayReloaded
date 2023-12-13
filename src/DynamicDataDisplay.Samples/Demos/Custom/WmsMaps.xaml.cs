using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.Maps.Servers.Network;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DynamicDataDisplay.Samples.Demos.Custom
{
	/// <summary>
	/// Interaction logic for Maps.xaml
	/// </summary>
	public partial class WmsMaps : Page
	{
		public WmsMaps()
		{
			InitializeComponent();

			((NumericAxis)plotter.MainHorizontalAxis).LabelProvider.LabelStringFormat = "{0}°";
			((NumericAxis)plotter.MainVerticalAxis).LabelProvider.LabelStringFormat = "{0}°";

			UIElement messageGrid = (UIElement)Resources["warningMessage"];
			plotter.MainCanvas.Children.Add(messageGrid);
		}

		public string ServerURL { get; set; } = @"http://ows.terrestris.de/osm/service";


		public string LayerName { get; set; } = "OSM-WMS";


		public string MaximumLevel { get; set; } = "15";

		private RelayCommandFactoryD3 _connectToServerCommand = new RelayCommandFactoryD3();
		public ICommand ConnectToServerCommand => _connectToServerCommand.GetCommand(() =>
		{
			if (!int.TryParse(MaximumLevel, out int level))
			{
				level = 4;
			}

			map.SourceTileServer = new WMSTileServer(ServerURL, LayerName, level);
        });
    }
}
