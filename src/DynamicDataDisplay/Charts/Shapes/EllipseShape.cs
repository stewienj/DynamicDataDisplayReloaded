using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.Charts
{
	public sealed class EllipseShape : ViewportShape
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EllipseShape"/> class.
		/// </summary>
		public EllipseShape() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="EllipseShape"/> class.
		/// </summary>
		/// <param name="bounds">The bounds.</param>    
		public EllipseShape(Rect bounds)
		{
			// X = longitude
			// Y = latitude
			// width = width
			// height = height
			Bounds = bounds;
		}

		public EllipseShape(double x, double y, double w, double h)
		{
			// X = longitude
			// Y = latitude
			// width = width
			// height = height
			Bounds = new Rect(x, y, w, h);
		}

		private DataRect rect = DataRect.Empty;
		public DataRect Bounds
		{
			get { return rect; }
			set
			{
				if (rect != value)
				{
					rect = value;
					UpdateUIRepresentation();
				}
			}
		}

		protected override void UpdateUIRepresentationCore()
		{
			var transform = Plotter.Viewport.Transform;

			Rect r = rect.DataToScreen(transform);

			// Rect is based off lower-left corner of rectangle.
			// Ellipse centre is midpoint.

			ellipseGeometry.Center = new Point(r.X, r.Y);
			ellipseGeometry.RadiusX = r.Width;
			ellipseGeometry.RadiusY = r.Height;

		}

		private EllipseGeometry ellipseGeometry = new EllipseGeometry();
		protected override Geometry DefiningGeometry
		{
			get { return ellipseGeometry; }
		}
	}
}

