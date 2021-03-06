﻿using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.Charts.Maps
{
	[Serializable]
	public class TileLoadResultEventArgs : EventArgs
	{
		public TileLoadResult Result { get; internal set; }
		public BitmapSource Image { get; internal set; }
		public Stream Stream { get; internal set; }
		public TileIndex ID { get; internal set; }
	}

	public enum TileLoadResult
	{
		Success,
		Failure
	}
}
