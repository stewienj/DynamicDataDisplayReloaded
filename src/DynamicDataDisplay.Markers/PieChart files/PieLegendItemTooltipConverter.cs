using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers
{
	public sealed class PieLegendItemTooltipConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length != 2)
				return DependencyProperty.UnsetValue;

			var tooltip = string.Format("{0} - {1}", values);
			return tooltip;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
