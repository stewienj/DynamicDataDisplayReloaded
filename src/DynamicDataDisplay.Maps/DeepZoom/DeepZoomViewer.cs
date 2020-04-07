using DynamicDataDisplay.Charts.Maps;

namespace DynamicDataDisplay.Maps.DeepZoom
{
	public class DeepZoomViewer : Map
	{
		private DeepZoomTileServer tileServer = new DeepZoomTileServer();

		public DeepZoomViewer()
		{
			SourceTileServer = tileServer;
			Mode = TileSystemMode.OnlineOnly;
			TileProvider = new DeepZoomTileProvider();

			ProportionsRestriction.ProportionRatio = 1;
		}

		public DeepZoomViewer(string imagePath)
			: this()
		{
			ImagePath = imagePath;
		}

		public string ImagePath
		{
			get { return tileServer.ImagePath; }
			set
			{
				tileServer.ImagePath = value;
			}
		}

	}
}
