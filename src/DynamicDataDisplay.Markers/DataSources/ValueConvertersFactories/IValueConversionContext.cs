using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	public interface IValueConversionContext
	{
		Plotter Plotter { get; }
	}
}
