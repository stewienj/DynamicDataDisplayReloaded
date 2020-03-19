using System;
using System.Globalization;
using System.Windows.Data;
using System.Xml;

namespace NewMarkersSample.Pages
{
	public class XmlTextToDoubleConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			XmlText text = (XmlText)value;
			return double.Parse(text.Data);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
