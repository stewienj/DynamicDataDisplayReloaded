using Microsoft.Research.DynamicDataDisplay;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers
{
	public class AngleToArcEndPointConverter : IValueConverter
	{
		#region IValueConverter Members

		const double maxAngle = 6.2831;
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double angle = (double)value;
			angle = angle.DegreesToRadians();

			if (angle > maxAngle)
				angle = maxAngle;

			double x = Math.Cos(angle);
			double y = 1 - Math.Sin(angle);

			return new Point(x, y);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
