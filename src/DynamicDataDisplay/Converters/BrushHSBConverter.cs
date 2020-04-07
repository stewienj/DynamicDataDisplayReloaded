using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicDataDisplay.Converters
{
	public sealed class BrushHSBConverter : IValueConverter
	{
		private double lightnessDelta = 1.0;
		public double LightnessDelta
		{
			get { return lightnessDelta; }
			set { lightnessDelta = value; }
		}

		private double saturationDelta = 1.0;
		public double SaturationDelta
		{
			get { return saturationDelta; }
			set { saturationDelta = value; }
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SolidColorBrush brush = value as SolidColorBrush;
			if (brush != null)
			{
				SolidColorBrush result = brush.ChangeLightness(lightnessDelta).ChangeSaturation(saturationDelta);
				return result;
			}
			else { return value; }
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
