using System;
using System.Globalization;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers
{
	public class AngleToIsLargeArcConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double angle = (double)value;

			return angle >= 180;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
