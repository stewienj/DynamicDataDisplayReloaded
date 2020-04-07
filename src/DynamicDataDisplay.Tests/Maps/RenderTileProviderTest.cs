using DynamicDataDisplay;
using DynamicDataDisplay.Maps.Charts.TiledRendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DynamicDataDisplay.Tests.Maps
{
	[TestClass]
	public class RenderTileProviderTest
	{
		[TestMethod]
		public void TestGetTiles()
		{
			RenderTileProvider provider = new RenderTileProvider();
			var tiles = provider.GetTilesForRegion(new DataRect(0, 0, 1, 1), provider.Level).ToArray();
		}
	}
}
