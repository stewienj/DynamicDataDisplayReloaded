using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using System;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class VerticalDateTimeAxis : DateTimeAxis
	{
		public VerticalDateTimeAxis()
		{
			Placement = AxisPlacement.Left;
			Restriction = new DateTimeVerticalAxisRestriction();
		}

		protected override void ValidatePlacement(AxisPlacement newPlacement)
		{
			if (newPlacement == AxisPlacement.Bottom || newPlacement == AxisPlacement.Top)
				throw new ArgumentException(Strings.Exceptions.VerticalAxisCannotBeHorizontal);
		}
	}
}
