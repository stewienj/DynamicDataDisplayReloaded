using System.Windows;

namespace DynamicDataDisplay.Markers
{
	public abstract class TemplateSelectorMarkerGenerator : MarkerGenerator
	{
		protected override FrameworkElement CreateMarkerCore(object dataItem)
		{
			var template = SelectTemplate(dataItem);
			var marker = (FrameworkElement)template.LoadContent();
			marker.DataContext = dataItem;
			return marker;
		}

		protected abstract DataTemplate SelectTemplate(object dataItem);
	}
}
