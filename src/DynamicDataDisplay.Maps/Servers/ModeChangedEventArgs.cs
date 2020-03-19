using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers
{
	public sealed class ModeChangedEventArgs : EventArgs
	{
		private readonly TileSystemMode mode;
		public ModeChangedEventArgs(TileSystemMode mode)
		{
			this.mode = mode;
		}

		public TileSystemMode Mode
		{
			get { return mode; }
		}
	}
}
