﻿namespace DynamicDataDisplay.Maps
{
	/// <summary>
	/// Represents a VirtualEarth Imagery Service Road tile server.
	/// </summary>
	public sealed class VEImageryRoadServer : VEImageryServerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VEImageryRoadServer"/> class.
		/// </summary>
		public VEImageryRoadServer()
		{
			MapStyle = VE.VESvc.MapStyle.Road;
			FileExtension = ".png";
			ServerName += "Road";
		}
	}
}
