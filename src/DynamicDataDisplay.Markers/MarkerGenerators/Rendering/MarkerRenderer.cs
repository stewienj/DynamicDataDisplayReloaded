using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Markers.MarkerGenerators.Rendering
{
	public abstract class MarkerRenderer : FrameworkElement
	{
		public abstract void Render(DrawingContext dc, CoordinateTransform transform);
	}
}
