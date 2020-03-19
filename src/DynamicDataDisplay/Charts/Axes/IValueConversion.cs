using System;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public interface IValueConversion<T>
	{
		Func<T, double> ConvertToDouble { get; set; }
		Func<double, T> ConvertFromDouble { get; set; }
	}
}
