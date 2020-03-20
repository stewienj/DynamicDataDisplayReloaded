using System;
using System.Collections.Generic;

namespace DynamicDataDisplay.RadioBand
{
	internal class RadioBandLineComparer : Comparer<RadioBandLine>
	{
		private static Lazy<RadioBandLineComparer> _instance = new Lazy<RadioBandLineComparer>(true);
		public static RadioBandLineComparer Instance => _instance.Value;

		public override int Compare(RadioBandLine x, RadioBandLine y)
		{
			// Return -1 or 1, don't return zero as we need this for a sorted Set which doesn't allow duplicates
			return (x.Start <= y.Start) ? -1 : 1;
		}

	}
}
