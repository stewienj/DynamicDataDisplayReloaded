using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace DynamicDataDisplay.SlimDX
{
	internal static class Global
	{
		public static Stream GetResourceStream(string name)
		{
			Assembly a = typeof(Global).Assembly;
			var resourceNames = a.GetManifestResourceNames();
			var matching = resourceNames.First(n => n.EndsWith(name));
			return a.GetManifestResourceStream(matching);
		}
		public static Uri MakePackUri(string relativeFile)
		{
			string uriString = "pack://application:,,,/" + AssemblyShortName + ";component/" + relativeFile;
			return new Uri(uriString);
		}

		private static string AssemblyShortName
		{
			get
			{
				if (_assemblyShortName == null)
				{
					Assembly a = typeof(Global).Assembly;

					// Pull out the short name.
					_assemblyShortName = a.ToString().Split(',')[0];
				}

				return _assemblyShortName;
			}
		}

		private static string _assemblyShortName;
	}
}
