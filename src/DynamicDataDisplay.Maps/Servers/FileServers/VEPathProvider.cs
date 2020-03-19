using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers.FileServers
{
	public sealed class VEPathProvider : TilePathProvider
	{
		public override string GetTilePath(TileIndex id)
		{
			StringBuilder builder = new StringBuilder();

			int minLevel = 1;

			checked
			{
				for (int level = minLevel; level <= id.Level; level++)
				{
					char ch = '0';
					int halfTilesNum = (int)Math.Pow(2, id.Level - level);
					if ((id.X & halfTilesNum) != 0)
						ch += (char)1;
					if ((id.Y & halfTilesNum) == 0)
						ch += (char)2;
					builder.Append(ch);
				}
			}

			return builder.ToString();
		}
	}
}
