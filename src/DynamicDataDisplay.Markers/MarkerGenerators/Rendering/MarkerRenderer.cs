using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.Markers.MarkerGenerators.Rendering
{
	public abstract class MarkerRenderer : FrameworkElement
	{
		public abstract void Render(DrawingContext dc, CoordinateTransform transform);
	}
}
