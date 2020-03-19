using System;
using System.Globalization;
using System.Windows.Data;

namespace NewMarkersSample.Pages
{
	public class TooltipConverter : IMultiValueConverter
	{
		public TooltipConverter()
		{
			FormatString = "{0}: {1}->{2}";
		}

		public string FormatString { get; set; }

		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			string formatString = FormatString.Replace('[', '{').Replace(']', '}');
			return string.Format(formatString, values);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
