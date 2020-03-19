using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.Markers
{
	public class DiamondMarker : PathMarker
	{
		protected override Geometry CreateGeometry()
		{
			PathGeometry geom = new PathGeometry();
			PathFigure figure = new PathFigure { StartPoint = new Point(0, 0), IsClosed = true, IsFilled = true };
			figure.Segments.Add(new LineSegment(new Point(0.5, 0.5), true));
			figure.Segments.Add(new LineSegment(new Point(0, 1), true));
			figure.Segments.Add(new LineSegment(new Point(-0.5, 0.5), true));
			geom.Figures.Add(figure);
			return geom;
		}
	}
}
