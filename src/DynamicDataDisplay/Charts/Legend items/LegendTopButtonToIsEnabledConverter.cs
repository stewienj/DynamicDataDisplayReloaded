using Microsoft.Research.DynamicDataDisplay.Converters;
using System;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public sealed class LegendTopButtonToIsEnabledConverter : GenericValueConverter<double>
	{
		public override object ConvertCore(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double verticalOffset = value;

			return verticalOffset > 0;
		}
	}
}
