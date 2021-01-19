using DynamicDataDisplay.SamplesDX9.Internals.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace DynamicDataDisplay.SamplesDX9.Internals
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

			string major = parts.Skip(0).FirstOrDefault() ?? "";
			string minor = parts.Skip(1).FirstOrDefault() ?? "";
			string revision = parts.Skip(2).FirstOrDefault() ?? "";
			ReleaseVersion result = new ReleaseVersion(major, minor, revision);
			return result;
		}
	}
}
