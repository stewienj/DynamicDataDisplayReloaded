using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9
{
	internal static class Global
	{
		public static string GetResourceText(string name)
		{
			Assembly a = typeof(Global).Assembly;
			var resourceNames = a.GetManifestResourceNames();
			var matching = resourceNames.First(n => n.EndsWith(name));
			using (var stream = a.GetManifestResourceStream(matching))
			{
				using(var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
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
