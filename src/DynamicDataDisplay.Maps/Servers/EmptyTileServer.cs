using DynamicDataDisplay.Charts.Maps;
using DynamicDataDisplay.Maps.Servers.Network;

namespace DynamicDataDisplay.Maps.Servers
{
	public class EmptyTileServer : SourceTileServer
	{
		#region ITileServer Members

		public override bool Contains(TileIndex id)
		{
			return false;
		}

		public override void BeginLoadImage(TileIndex id) { }

		protected override string GetCustomName()
		{
			return "Empty";
		}

		#endregion
	}
}
