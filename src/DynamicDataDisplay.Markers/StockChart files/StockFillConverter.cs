using DynamicDataDisplay.Converters;
using System;
using System.Globalization;
using System.Windows.Media;

namespace DynamicDataDisplay.Markers
{
	public sealed class StockFillConverter : TwoValuesMultiConverter<double, double>
	{
		private Brush positiveFill;
		public Brush PositiveFill
		{
			get { return positiveFill; }
			set { positiveFill = value; }
		}

		private Brush negativeFill;
		public Brush NegativeFill
		{
			get { return negativeFill; }
			set { negativeFill = value; }
		}

		protected override object ConvertCore(double value1, double value2, Type targetType, object parameter, CultureInfo culture)
		{
			double open;
			double close;
			open = value1;
			close = value2;

			if (open < close)
				return positiveFill;
			else
				return negativeFill;
		}
	}
}
