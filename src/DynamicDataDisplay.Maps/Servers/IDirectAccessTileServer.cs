using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.Charts.Maps
{
	public interface IDirectAccessTileServer : ITileServer
	{
		BitmapSource this[TileIndex id] { get; }
	}
}
