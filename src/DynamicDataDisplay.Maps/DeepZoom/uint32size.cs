﻿using System.Xml.Serialization;

namespace DynamicDataDisplay.Maps.DeepZoom
{
	public struct uint32size
	{
		/// <summary>
		/// The width of the image.
		/// </summary>
		[XmlAttribute]
		public ulong Width { get; set; }
		/// <summary>
		/// The height of the image.
		/// </summary>
		[XmlAttribute]
		public ulong Height { get; set; }
	}
}
