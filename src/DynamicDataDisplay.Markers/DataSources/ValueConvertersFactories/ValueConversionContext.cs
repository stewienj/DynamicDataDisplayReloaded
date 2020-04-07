using DynamicDataDisplay;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	internal sealed class ValueConversionContext : IValueConversionContext
	{
		#region IValueConversionContext Members

		public Plotter Plotter { get; set; }

		#endregion
	}
}
