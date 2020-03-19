using Microsoft.Research.DynamicDataDisplay.Charts;
using System;


namespace Microsoft.Research.DynamicDataDisplay
{
	public class TimeChartPlotter : ChartPlotter
	{
		public TimeChartPlotter()
		{
			MainHorizontalAxis = new HorizontalDateTimeAxis();
		}

		public void SetHorizontalAxisMapping(Func<double, DateTime> fromDouble, Func<DateTime, double> toDouble)
		{
			if (fromDouble == null)
				throw new ArgumentNullException("fromDouble");
			if (toDouble == null)
				throw new ArgumentNullException("toDouble");


			HorizontalDateTimeAxis axis = (HorizontalDateTimeAxis)MainHorizontalAxis;
			axis.ConvertFromDouble = fromDouble;
			axis.ConvertToDouble = toDouble;
		}

		public void SetHorizontalAxisMapping(double min, DateTime minDate, double max, DateTime maxDate)
		{
			HorizontalDateTimeAxis axis = (HorizontalDateTimeAxis)MainHorizontalAxis;

			axis.SetConversion(min, minDate, max, maxDate);
		}
	}
}
