using DynamicDataDisplay.Converters;
using System;
using System.Globalization;

namespace DynamicDataDisplay.Markers
{
	public sealed class AcceptableRangeMultiConverter : TwoValuesMultiConverter<double, double>
	{
		protected override object ConvertCore(double value1, double value2, Type targetType, object parameter, CultureInfo culture)
		{
			double yMin = value1;
			double yMax = value2;

			return yMax > yMin ? yMax - yMin : 0;
		}
	}
}
