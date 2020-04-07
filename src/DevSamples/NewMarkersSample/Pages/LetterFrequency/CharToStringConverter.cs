using DynamicDataDisplay.Converters;
using System;
using System.Globalization;

namespace NewMarkersSample.Pages
{
	public class CharToStringConverter : GenericValueConverter<char>
	{
		public override object ConvertCore(char value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.ToString();
		}
	}
}
