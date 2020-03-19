using System.Windows.Media.Imaging;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public interface IDirectAccessTileServer : ITileServer
	{
		BitmapSource this[TileIndex id] { get; }
	}
}
