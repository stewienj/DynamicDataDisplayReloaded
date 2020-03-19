using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Windows.Media.Imaging;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers
{
	public class LRUMemoryCache : LRUMemoryCacheBase<BitmapSource>, ITileSystem
	{
		public LRUMemoryCache(string name)
			: base(name)
		{

		}
	}
}
