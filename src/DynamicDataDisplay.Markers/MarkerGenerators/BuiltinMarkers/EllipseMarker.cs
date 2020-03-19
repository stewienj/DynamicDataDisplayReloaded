using System.Windows.Shapes;

namespace DynamicDataDisplay.Markers
{
	public class EllipseMarker : ShapeMarker
	{
		public EllipseMarker() { }

		protected override Shape CreateShape()
		{
			return new Ellipse();
		}
	}
}
