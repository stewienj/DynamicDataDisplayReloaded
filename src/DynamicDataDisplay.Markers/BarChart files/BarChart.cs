using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
using System.Windows;

namespace DynamicDataDisplay.Markers
{
	public class BarChart : DevMarkerChart
	{
		static BarChart()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarChart), new FrameworkPropertyMetadata(typeof(BarChart)));
		}

		public BarChart()
		{
			PropertyMappings[DependentValuePathProperty] = ViewportPanel.ViewportWidthProperty;
			MarkerBuilder = new TemplateMarkerGenerator();
		}
	}
}
