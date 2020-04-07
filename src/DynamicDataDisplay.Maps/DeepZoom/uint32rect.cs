using System.Xml.Serialization;

namespace DynamicDataDisplay.Maps.DeepZoom
{
	public struct uint32rect
	{
		/// <summary>
		/// X coordinate of the upper-left corner of the rectangle.
		/// </summary>
		[XmlAttribute]
		public ulong X { get; set; }
		/// <summary>
		/// Y coordinate of the upper-left corner of the rectangle.
		/// </summary>
		[XmlAttribute]
		public ulong Y { get; set; }
		/// <summary>
		/// Width of the rectangle.
		/// </summary>
		[XmlAttribute]
		public ulong Width { get; set; }
		/// <summary>
		/// Height of the rectangle.
		/// </summary>
		[XmlAttribute]
		public ulong Height { get; set; }
	}
}
