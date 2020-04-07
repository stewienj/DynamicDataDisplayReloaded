using DynamicDataDisplay.DataSources;
using System;

namespace DynamicDataDisplay
{
	public interface IOneDimensionalChart
	{
		IPointDataSource DataSource { get; set; }
		event EventHandler DataChanged;
	}
}
