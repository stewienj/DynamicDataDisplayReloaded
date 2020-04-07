using System;

namespace DynamicDataDisplay.Charts
{
	public sealed class DateTimeToDoubleConversion
	{
		public DateTimeToDoubleConversion(double min, DateTime minDate, double max, DateTime maxDate)
		{
			this.min = min;
			this.length = max - min;
			this.ticksMin = minDate.Ticks;
			this.ticksLength = maxDate.Ticks - ticksMin;
		}

		private double min;
		private double length;
		private long ticksMin;
		private long ticksLength;

		public DateTime FromDouble(double d)
		{
			double ratio = (d - min) / length;
			long tick = (long)(ticksMin + ticksLength * ratio);

			tick = MathHelper.Clamp(tick, DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks);

			return new DateTime(tick);
		}

		public double ToDouble(DateTime dt)
		{
			double ratio = (dt.Ticks - ticksMin) / (double)ticksLength;
			return min + ratio * length;
		}
	}
}
