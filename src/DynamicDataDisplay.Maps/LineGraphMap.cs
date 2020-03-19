using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
	/// <summary>
	/// This renders a line 3 times over, at 360 degrees each way from the original
	/// </summary>
	public class LineGraphMap : LineGraph
	{
		private TranslateTransform _offsetLeft = new TranslateTransform();
		private TranslateTransform _offsetRight = new TranslateTransform();

		protected override void UpdateCore()
		{
			if (DataSource == null) return;
			if (Plotter == null) return;
			if (!IsEnabled) return;

			Rect output = Viewport.Output;
			var transform = GetTransform();

			if (FilteredPoints == null || !(transform.DataTransform is IdentityTransform))
			{
				IEnumerable<Point> points = GetPoints();
				if (!points.Any())
					return;

				var bounds = BoundsHelper.GetViewportBounds(points, transform.DataTransform);
				if (bounds.Width >= 0 && bounds.Height >= 0)
				{
					bounds = new DataRect(bounds.XMin - 360.0, bounds.YMin, bounds.Width + 720.0, bounds.Height);
				}
				Viewport2D.SetContentBounds(this, bounds);

				// getting new value of transform as it could change after calculating and setting content bounds.
				transform = GetTransform();
				List<Point> transformedPoints = transform.DataToScreenAsList(points);

				var screenOffset = transform.DataToScreen(new Point(360, 0));

				// Analysis and filtering of unnecessary points
				FilteredPoints = new FakePointList(transformedPoints, double.NegativeInfinity, double.PositiveInfinity);
				if (ProvideVisiblePoints)
				{
					List<Point> viewportPointsList = null;
					viewportPointsList = new List<Point>(transformedPoints.Count);
					if (transform.DataTransform is IdentityTransform)
					{
						viewportPointsList.AddRange(points);
					}
					else
					{
						var viewportPoints = points.DataToViewport(transform.DataTransform);
						viewportPointsList.AddRange(viewportPoints);
					}
					SetVisiblePoints(this, new ReadOnlyCollection<Point>(viewportPointsList));
				}
				Offset = new Vector();
			}
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state)
		{
			if (DataSource == null) return;
			if (!IsEnabled) return;

			if (FilteredPoints?.HasPoints ?? false)
			{
				using (StreamGeometryContext context = streamGeometry.Open())
				{
					context.BeginFigure(FilteredPoints.StartPoint, false, false);
					context.PolyLineTo(FilteredPoints, true, SmoothLinesJoin);
				}

				Brush brush = null;
				Pen pen = LinePen;

				bool isTranslated = IsTranslated;
				if (isTranslated)
				{
					dc.PushTransform(new TranslateTransform(Offset.X, Offset.Y));
				}
				dc.DrawGeometry(brush, pen, streamGeometry);

				var transform = Viewport.Transform;
				var scale = (transform.DataToScreen(new Point(1, 0)).X - transform.DataToScreen(new Point(0, 0)).X);
				_offsetLeft.X = scale * -360.0;
				_offsetRight.X = scale * 360.0;

				dc.PushTransform(_offsetLeft);
				dc.DrawGeometry(brush, pen, streamGeometry);
				dc.Pop();
				dc.PushTransform(_offsetRight);
				dc.DrawGeometry(brush, pen, streamGeometry);
				dc.Pop();

				if (isTranslated)
				{
					dc.Pop();
				}
			}
		}
	}
}
