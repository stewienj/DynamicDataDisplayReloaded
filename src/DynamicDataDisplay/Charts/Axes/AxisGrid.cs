using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Draws grid over viewport. Number of 
	/// grid lines depends on Plotter's MainHorizontalAxis and MainVerticalAxis ticks.
	/// </summary>
	public class AxisGrid : ContentControl, IPlotterElement
	{
		static AxisGrid()
		{
			Type thisType = typeof(AxisGrid);
			Panel.ZIndexProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(-1));
		}

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void BeginTicksUpdate()
		{
		}
		public void EndTicksUpdate()
		{
			UpdateUIRepresentation();
		}

		public MinorTickInfo<double>[] MinorHorizontalTicks { get; set; }

		public MinorTickInfo<double>[] MinorVerticalTicks { get; set; }

		public MajorTickInfo<double>[] HorizontalTicks { get; set; }

		public MajorTickInfo<double>[] VerticalTicks { get; set; }


		private bool _drawVerticalsShaded = false;
		/// <summary>
		/// Gets or sets a value indicating whether to draw vertical major ticks as grey bars
		/// </summary>
		/// <value><c>true</c> if draw vertical ticks; otherwise, <c>false</c>.</value>
		public bool DrawVerticalsShaded
		{
			get { return _drawVerticalsShaded; }
			set
			{
				if (_drawVerticalsShaded != value)
				{
					_drawVerticalsShaded = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool _drawHorizontalsShaded = false;
		/// <summary>
		/// Gets or sets a value indicating whether to draw horizontal major ticks as grey bars
		/// </summary>
		/// <value><c>true</c> if draw horizontal ticks; otherwise, <c>false</c>.</value>
		public bool DrawHorizontalsShaded
		{
			get { return _drawHorizontalsShaded; }
			set
			{
				if (_drawHorizontalsShaded != value)
				{
					_drawHorizontalsShaded = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool drawVerticalTicks = true;
		/// <summary>
		/// Gets or sets a value indicating whether to draw vertical tick lines.
		/// </summary>
		/// <value><c>true</c> if draw vertical ticks; otherwise, <c>false</c>.</value>
		public bool DrawVerticalTicks
		{
			get { return drawVerticalTicks; }
			set
			{
				if (drawVerticalTicks != value)
				{
					drawVerticalTicks = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool drawHorizontalTicks = true;
		/// <summary>
		/// Gets or sets a value indicating whether to draw horizontal tick lines.
		/// </summary>
		/// <value><c>true</c> if draw horizontal ticks; otherwise, <c>false</c>.</value>
		public bool DrawHorizontalTicks
		{
			get { return drawHorizontalTicks; }
			set
			{
				if (drawHorizontalTicks != value)
				{
					drawHorizontalTicks = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool drawHorizontalMinorTicks = false;
		/// <summary>
		/// Gets or sets a value indicating whether to draw horizontal minor ticks.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if draw horizontal minor ticks; otherwise, <c>false</c>.
		/// </value>
		public bool DrawHorizontalMinorTicks
		{
			get { return drawHorizontalMinorTicks; }
			set
			{
				if (drawHorizontalMinorTicks != value)
				{
					drawHorizontalMinorTicks = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool drawVerticalMinorTicks = false;
		/// <summary>
		/// Gets or sets a value indicating whether to draw vertical minor ticks.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if draw vertical minor ticks; otherwise, <c>false</c>.
		/// </value>
		public bool DrawVerticalMinorTicks
		{
			get { return drawVerticalMinorTicks; }
			set
			{
				if (drawVerticalMinorTicks != value)
				{
					drawVerticalMinorTicks = value;
					UpdateUIRepresentation();
				}
			}
		}

		private double gridBrushThickness = 1;

		private Path linesPath = new Path();
		private Path rectanglesPath = new Path();
		private Canvas canvas = new Canvas();
		/// <summary>
		/// Initializes a new instance of the <see cref="AxisGrid"/> class.
		/// </summary>
		public AxisGrid()
		{
			IsHitTestVisible = false;

			canvas.ClipToBounds = true;

			linesPath.Stroke = Brushes.LightGray;
			linesPath.StrokeThickness = gridBrushThickness;
			rectanglesPath.Fill = new SolidColorBrush(Color.FromRgb(0xF8, 0xF8, 0xF8));

			Content = canvas;
		}

		private readonly ResourcePool<LineGeometry> lineGeometryPool = new ResourcePool<LineGeometry>();
		private readonly ResourcePool<Line> linePool = new ResourcePool<Line>();

		private readonly ResourcePool<RectangleGeometry> rectangleGeometryPool = new ResourcePool<RectangleGeometry>();
		private readonly ResourcePool<Rectangle> rectanglePool = new ResourcePool<Rectangle>();


		protected virtual void UpdateUIRepresentation()
		{
			foreach (UIElement item in canvas.Children)
			{
				linePool.PutIfType(item);
				rectanglePool.PutIfType(item);
			}
			GeometryGroup prevLinesGroup = linesPath.Data as GeometryGroup;
			if (prevLinesGroup != null)
			{
				foreach (Geometry geometry in prevLinesGroup.Children)
				{
					lineGeometryPool.PutIfType(geometry);
				}
			}
			GeometryGroup prevRectanglesGroup = linesPath.Data as GeometryGroup;
			if (prevRectanglesGroup != null)
			{
				foreach (Geometry geometry in prevRectanglesGroup.Children)
				{
					rectangleGeometryPool.PutIfType(geometry);
				}
			}

			canvas.Children.Clear();
			Size size = RenderSize;

			double minY = 0;
			double maxY = size.Height;
			double minX = 0;
			double maxX = size.Width;


			GeometryGroup rectanglesGroup = new GeometryGroup();
			if (HorizontalTicks != null && _drawHorizontalsShaded)
			{
				for (int i = 0; i < HorizontalTicks.Length; i += 2)
				{
					// Want to check the first 2 points to work out our shading order
					if (i == 0 && HorizontalTicks.Length > 1)
					{
						// rectangle between the first point and the next point will be grey, if first point is
						// odd then shift back 1 step
						double dx = Math.Abs(HorizontalTicks[1].Tick - HorizontalTicks[0].Tick);
						// Test if 2nd point is even and if so then move back 1 tick
						if (dx > 0 && Math.Abs((int)(HorizontalTicks[1].Tick / dx)) % 2 == 0)
						{
							i = -1;
						}
					}

					// TODO try to make rectanlge even on the left to odd on the right
					// Get the difference, divide each by the difference. Get the int....
					double screenXStart = (i >= 0) ? HorizontalTicks[i].Value : minX;
					double screenXEnd = (i + 1) < HorizontalTicks.Length ? HorizontalTicks[i + 1].Value : maxX;
					RectangleGeometry rectangle = rectangleGeometryPool.GetOrCreate();
					rectangle.Rect = PointsToRect(screenXStart, screenXEnd, minY, maxY);
					rectanglesGroup.Children.Add(rectangle);
				}
			}

			DrawMinorHorizontalTicks();
			DrawMinorVerticalTicks();


			GeometryGroup linesGroup = new GeometryGroup();
			if (HorizontalTicks != null && drawHorizontalTicks)
			{
				for (int i = 0; i < HorizontalTicks.Length; i++)
				{
					double screenX = HorizontalTicks[i].Value;
					LineGeometry line = lineGeometryPool.GetOrCreate();
					line.StartPoint = new Point(screenX, minY);
					line.EndPoint = new Point(screenX, maxY);
					linesGroup.Children.Add(line);
				}
			}

			if (VerticalTicks != null && drawVerticalTicks)
			{

				for (int i = 0; i < VerticalTicks.Length; i++)
				{
					double screenY = VerticalTicks[i].Value;
					LineGeometry line = lineGeometryPool.GetOrCreate();
					line.StartPoint = new Point(minX, screenY);
					line.EndPoint = new Point(maxX, screenY);
					linesGroup.Children.Add(line);
				}
			}

			canvas.Children.Add(rectanglesPath);
			canvas.Children.Add(linesPath);
			linesPath.Data = linesGroup;
			rectanglesPath.Data = rectanglesGroup;
		}

		private Rect PointsToRect(double x1, double x2, double y1, double y2)
		{
			var xs = (new double[] { x1, x2 }).OrderBy(x => x).ToArray();
			var ys = (new double[] { y1, y2 }).OrderBy(y => y).ToArray();

			var dx = xs[1] - xs[0];
			return new Rect(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
		}

		private void DrawMinorVerticalTicks()
		{
			Size size = RenderSize;
			if (MinorVerticalTicks != null && drawVerticalMinorTicks)
			{
				double minX = 0;
				double maxX = size.Width;

				for (int i = 0; i < MinorVerticalTicks.Length; i++)
				{
					double screenY = MinorVerticalTicks[i].Tick;
					if (screenY < 0)
						continue;
					if (screenY > size.Height)
						continue;

					if (screenY.IsFinite() && screenY.IsNotNaN())
					{
						Line line = linePool.GetOrCreate();

						line.Y1 = screenY;
						line.Y2 = screenY;
						line.X1 = minX;
						line.X2 = maxX;
						line.Stroke = Brushes.LightGray;
						line.StrokeThickness = MinorVerticalTicks[i].Value * gridBrushThickness;

						canvas.Children.Add(line);
					}
				}
			}
		}

		private void DrawMinorHorizontalTicks()
		{
			Size size = RenderSize;
			if (MinorHorizontalTicks != null && drawHorizontalMinorTicks)
			{
				double minY = 0;
				double maxY = size.Height;

				for (int i = 0; i < MinorHorizontalTicks.Length; i++)
				{
					double screenX = MinorHorizontalTicks[i].Tick;
					if (screenX < 0)
						continue;
					if (screenX > size.Width)
						continue;

					if (screenX.IsFinite() && screenX.IsNotNaN())
					{
						Line line = linePool.GetOrCreate();
						line.X1 = screenX;
						line.X2 = screenX;
						line.Y1 = minY;
						line.Y2 = maxY;
						line.Stroke = Brushes.LightGray;
						line.StrokeThickness = MinorHorizontalTicks[i].Value * gridBrushThickness;

						canvas.Children.Add(line);
					}
				}
			}
		}

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			plotter.CentralGrid.Children.Add(this);
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			plotter.CentralGrid.Children.Remove(this);
			this.plotter = null;
		}

		private Plotter plotter;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}