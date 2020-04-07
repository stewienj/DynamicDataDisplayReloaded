using DynamicDataDisplay.Charts;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers.DataSources.ValueConverters
{
	public sealed class DateTimeValueConverter : IValueConverter
	{
		public DateTimeValueConverter(IValueConversion<DateTime> conversion)
		{
			if (conversion == null)
				throw new ArgumentNullException("conversion");

			this.conversion = conversion;
		}

		private readonly IValueConversion<DateTime> conversion;

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			DateTime dateTime = (DateTime)value;
			double result = conversion.ConvertToDouble(dateTime);
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double number = (double)value;
			return conversion.ConvertFromDouble(number);
		}

		#endregion
	}
}
