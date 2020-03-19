using Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals
{
	public class VersionTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string str = (string)value;

			string[] parts = str.Split('.');

			int major = int.Parse(parts[0], culture);
			int minor = int.Parse(parts[1], culture);
			int revision = int.Parse(parts[2], culture);
			ReleaseVersion result = new ReleaseVersion(major, minor, revision);
			return result;
		}
	}
}
