using System;
using System.Windows;
using System.Windows.Data;

namespace DynamicDataDisplay.Charts.Markers
{
	public class HorizontalColumnChart : DevMarkerChart
	{
		public HorizontalColumnChart()
		{
			ResourceDictionary dict = new ResourceDictionary
			{
				Source = new Uri("/DynamicDataDisplay.Markers;component/Themes/Generic.xaml", UriKind.Relative)
			};

			viewportYBinding = (Binding)dict["columnChartIndexBinding"];
		}

		private Binding viewportWidthBinding = new Binding();
		private Binding viewportYBinding;
		protected override void AddCommonBindings(FrameworkElement marker)
		{
			base.AddCommonBindings(marker);

			marker.SetValue(ViewportPanel.XProperty, 0.0);
			marker.SetBinding(ViewportPanel.ViewportWidthProperty, viewportWidthBinding);
			marker.SetValue(ViewportPanel.ViewportHeightProperty, 0.85);
			marker.SetValue(ViewportPanel.ViewportHorizontalAlignmentProperty, HorizontalAlignment.Left);
			marker.SetBinding(ViewportPanel.YProperty, viewportYBinding);
		}
	}
}
