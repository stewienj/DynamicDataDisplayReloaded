// $File: //depot/WMS/WMS Overview/Wms.Client/ExtensionMap.cs $ $Revision: #1 $ $Change: 20 $ $DateTime: 2004/05/23 23:42:06 $

using System;

namespace Wms.Client
{
	/// <summary>
	/// Converts html content types to file extension suffixes.
	/// There is undoubtedly a way to do this via the Windows API,
	/// but I don't know what it is. ' Will add to the TODO list.
	/// </summary>
	public class ExtensionMap
	{
		private class ExtensionType
		{
			public string Format;
			public string Extension;

			public ExtensionType(string format, string extension)
			{
				this.Format = format;
				this.Extension = extension;
			}
		}

		private static ExtensionType[] extensionMappings = new ExtensionType[]
		{
			// These are the major types one's likely to encounter with WMS.
			new ExtensionType("image/gif", ".gif"),
			new ExtensionType("image/tiff", ".tiff"),
			new ExtensionType("image/png", ".png"),
			new ExtensionType("image/bmp", ".bmp"),
			new ExtensionType("image/jpeg", ".jpg"),
			new ExtensionType("application/xml", ".xml"),
			new ExtensionType("text/xml", ".xml"),
			new ExtensionType("application/vnd.ogc.wms_xml", ".xml"),
			new ExtensionType("application/vnd.ogc.gml", ".gml"),
			new ExtensionType("application/vnd.ogc.se+xml", ".xml"),
			new ExtensionType("application/vnd.ogc.se_xml", ".xml")
		};

		public static string AddSuffixToPath(string path, string contentType)
		{
			if (path == null)
				return null;

			foreach (ExtensionType em in extensionMappings)
			{
				if (contentType.Equals(em.Format, StringComparison.Ordinal))
				{
					return path + em.Extension;
				}
			}
			return path;
		}
	}
}
