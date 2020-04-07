using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DynamicDataDisplay.Charts.Markers
{
	public class ColumnChart : DevMarkerChart
	{
		static ColumnChart()
		{
			Type thisType = typeof(ColumnChart);
			MarkerTemplateProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(CreateDefaultColumnTemplate()));
		}

		private static object CreateDefaultColumnTemplate()
		{
			DataTemplate template = new DataTemplate();
			FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Rectangle));
			factory.SetValue(Shape.FillProperty, Brushes.Green);
			template.VisualTree = factory;

			return template;
		}

		public ColumnChart()
		{
			ResourceDictionary dict = new ResourceDictionary
			{
				Source = new Uri("/DynamicDataDisplay.Markers;component/Themes/Generic.xaml", UriKind.Relative)
			};

			viewportXBinding = (Binding)dict["columnChartIndexBinding"];
		}

		private Binding viewportHeightBinding = new Binding();
		private Binding viewportXBinding;
		protected override void AddCommonBindings(FrameworkElement marker)
		{
			base.AddCommonBindings(marker);

			marker.SetValue(ViewportPanel.YProperty, 0.0);

			if (!string.IsNullOrEmpty(DependentValuePath))
			{
				viewportHeightBinding = new Binding(DependentValuePath);
			}

			marker.SetBinding(ViewportPanel.ViewportHeightProperty, viewportHeightBinding);

			if (ViewportPanel.GetViewportWidth(marker).IsNaN())
				marker.SetValue(ViewportPanel.ViewportWidthProperty, 0.85);
			marker.SetValue(ViewportPanel.ViewportVerticalAlignmentProperty, VerticalAlignment.Bottom);
			marker.SetBinding(ViewportPanel.XProperty, viewportXBinding);
		}
	}
}
