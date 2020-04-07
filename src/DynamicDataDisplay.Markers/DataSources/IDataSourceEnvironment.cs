using DynamicDataDisplay;

namespace DynamicDataDisplay.Markers.DataSources
{
	public interface IDataSourceEnvironment
	{
		Plotter2D Plotter { get; }
		bool FirstDraw { get; }
	}
}
