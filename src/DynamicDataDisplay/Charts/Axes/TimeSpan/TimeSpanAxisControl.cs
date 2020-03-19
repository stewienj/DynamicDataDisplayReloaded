﻿using System;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class TimeSpanAxisControl : AxisControl<TimeSpan>
	{
		public TimeSpanAxisControl()
		{
			LabelProvider = new TimeSpanLabelProvider();
			TicksProvider = new TimeSpanTicksProvider();

			ConvertToDouble = time => time.Ticks;

			Range = new Range<TimeSpan>(new TimeSpan(), new TimeSpan(1, 0, 0));
		}
	}
}
