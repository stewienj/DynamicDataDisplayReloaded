using System;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Utility
{
	[ValueConversion(typeof(string), typeof(Visibility))]
	public class StringValidVisibilityConverter : IValueConverter
	{
		private static Lazy<StringValidVisibilityConverter> _instance = new Lazy<StringValidVisibilityConverter>(true);

		public static StringValidVisibilityConverter Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		//-------------------------------------------------------------------------
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string v = value as string;
			return string.IsNullOrEmpty(v) ? Visibility.Collapsed : Visibility.Visible;
		}

		//-------------------------------------------------------------------------
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
