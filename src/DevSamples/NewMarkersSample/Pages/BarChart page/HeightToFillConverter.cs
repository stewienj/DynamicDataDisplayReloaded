using System;
using System.Windows.Data;
using System.Windows.Media;

namespace NewMarkersSample.Pages
{
	public class HeightToFillConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double d = (double)value;
			if (d > 3.75) return Brushes.Brown;
			else if (d > 3.5) return Brushes.IndianRed;
			else if (d > 3.25) return Brushes.BurlyWood;
			else return Brushes.CadetBlue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
