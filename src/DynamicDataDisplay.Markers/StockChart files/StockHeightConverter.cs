using System;
using System.Globalization;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers
{
	public sealed class StockHeightConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double high;
			double low;
			if (values.Length == 2)
			{
				if (values[0] is double)
				{
					high = (double)values[0];

					if (values[1] is double)
					{
						low = (double)values[1];

						return high - low;
					}
				}
			}

			return 0.0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
