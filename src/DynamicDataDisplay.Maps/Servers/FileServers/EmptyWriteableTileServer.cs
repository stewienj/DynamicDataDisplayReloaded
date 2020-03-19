using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.IO;
using System.Windows.Media.Imaging;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers
{
	public class EmptyWriteableTileServer : EmptyTileServer, ITileStore, IWriteableTileServer
	{
		#region ITileStore Members

		protected override string GetCustomName()
		{
			return "Empty writeable";
		}

		public void BeginSaveImage(TileIndex id, BitmapSource image, Stream stream)
		{
			// do nothing
		}

		public void Clear()
		{
			// do nothing
		}

		#endregion
	}
}
