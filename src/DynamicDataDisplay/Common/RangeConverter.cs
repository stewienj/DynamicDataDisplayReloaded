using DynamicDataDisplay.Charts;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace DynamicDataDisplay.Common
{
	public sealed class RangeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return (destinationType == typeof(string)) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				throw base.GetConvertFromException(value);
			}

			string source = value as string;
			if (source != null)
			{
				var parts = source.Split("-—".ToArray());
				var minStr = parts[0];
				var maxStr = parts[1];

				int minInt32 = 0;
				double minDouble = 0;
				DateTime minDateTime = DateTime.Now;
				if (int.TryParse(minStr, NumberStyles.Integer, culture, out minInt32))
				{
					int maxInt32 = int.Parse(maxStr, NumberStyles.Integer, culture);

					return new Range<int>(minInt32, maxInt32);
				}
				else if (double.TryParse(minStr, NumberStyles.Float, culture, out minDouble))
				{
					double maxDouble = double.Parse(maxStr, NumberStyles.Float, culture);
					return new Range<double>(minDouble, maxDouble);
				}
				else if (DateTime.TryParse(minStr, culture, DateTimeStyles.None, out minDateTime))
				{
					DateTime maxDateTime = DateTime.Parse(maxStr, culture);
					return new Range<DateTime>(minDateTime, maxDateTime);
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType != null && value is DataRect)
			{
				DataRect rect = (DataRect)value;
				if (destinationType == typeof(string))
				{
					return rect.ConvertToString(null, culture);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
