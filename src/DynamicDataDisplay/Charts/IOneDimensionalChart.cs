using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;

namespace Microsoft.Research.DynamicDataDisplay
{
	public interface IOneDimensionalChart
	{
		IPointDataSource DataSource { get; set; }
		event EventHandler DataChanged;
	}
}
