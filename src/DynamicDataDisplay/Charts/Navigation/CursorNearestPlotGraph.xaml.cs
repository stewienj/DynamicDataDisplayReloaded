using DynamicDataDisplay.Common.Auxiliary.DataSearch;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Adds to ChartPlotter two crossed lines, bound to mouse cursor position, and two labels near axes with mouse position in its text.
	/// </summary>
	public partial class CursorNearestPlotGraph : ContentGraph
	{
		private NearestPointSearch2D _nearestPointSearch = new NearestPointSearch2D();
		/// <summary>
		/// Initializes a new instance of the <see cref="CursorNearestPlotGraph"/> class.
		/// </summary>
		public CursorNearestPlotGraph()
		{
			_nearestPointSearch.NearestPointUpdated += _nearestPointSearch_NearestPointUpdated;
			this.SnapsToDevicePixels = true;
			InitializeComponent();
		}

		private void _nearestPointSearch_NearestPointUpdated(object sender, NearestPointSearchArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				Position = e.NearestPoint;
				labelTextBlock.Text = e.NearestLine;
				labelTextBlock.Visibility = string.IsNullOrWhiteSpace(e.NearestLine) ? Visibility.Collapsed : Visibility.Visible;

			});
		}

		#region Plotter

		protected override void OnPlotterAttached()
		{
			_nearestPointSearch.ContentBoundsHosts = Plotter2D.Viewport.ContentBoundsHosts;

			UIElement parent = (UIElement)Parent;

			parent.MouseMove += parent_MouseMove;
			parent.MouseEnter += Parent_MouseEnter;
			parent.MouseLeave += Parent_MouseLeave;
			UpdateUIRepresentation();
		}


		protected override void OnPlotterDetaching()
		{
			UIElement parent = (UIElement)Parent;

			parent.MouseMove -= parent_MouseMove;
			parent.MouseEnter -= Parent_MouseEnter;
			parent.MouseLeave -= Parent_MouseLeave;
		}

		#endregion

		private bool autoHide = true;
		/// <summary>
		/// Gets or sets a value indicating whether to hide automatically cursor lines when mouse leaves plotter.
		/// </summary>
		/// <value><c>true</c> if auto hide; otherwise, <c>false</c>.</value>
		public bool AutoHide
		{
			get { return autoHide; }
			set { autoHide = value; }
		}

		private void Parent_MouseEnter(object sender, MouseEventArgs e)
		{
			if (autoHide)
			{
				circleGrid.Visibility = Visibility.Visible;
			}
		}

		private void Parent_MouseLeave(object sender, MouseEventArgs e)
		{
			if (autoHide)
			{
				circleGrid.Visibility = Visibility.Hidden;
			}
		}

		private void parent_MouseMove(object sender, MouseEventArgs e)
		{
			if (autoHide)
			{
				circleGrid.Visibility = Visibility.Visible;
			}
			if (Plotter2D != null && Plotter2D.IsHitTestVisible)
			{
				Point mousePos = Mouse.GetPosition(this);
				CoordinateTransform transform = Plotter2D.Viewport.Transform;
				DataRect visible = Plotter2D.Viewport.Visible;
				Rect output = Plotter2D.Viewport.Output;

				if (!output.Contains(mousePos))
				{
					if (autoHide)
					{
						circleGrid.Visibility = Visibility.Hidden;
					}
					return;
				}

				Vector screenDistance = new Vector(bigCircle.Width * 0.5, bigCircle.Height * 0.5);
				_nearestPointSearch.UpdatePositionInData(mousePos, screenDistance, transform);
			}
			else
			{
				circleGrid.Visibility = Visibility.Hidden;
			}
		}

		protected override void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e)
		{
			UpdateUIRepresentation();
		}

		private string customXFormat = null;
		/// <summary>
		/// Gets or sets the custom format string of x label.
		/// </summary>
		/// <value>The custom X format.</value>
		public string CustomXFormat
		{
			get { return customXFormat; }
			set
			{
				if (customXFormat != value)
				{
					customXFormat = value;
					UpdateUIRepresentation();
				}
			}
		}

		private string customYFormat = null;
		/// <summary>
		/// Gets or sets the custom format string of y label.
		/// </summary>
		/// <value>The custom Y format.</value>
		public string CustomYFormat
		{
			get { return customYFormat; }
			set
			{
				if (customYFormat != value)
				{
					customYFormat = value;
					UpdateUIRepresentation();
				}
			}
		}

		private Func<double, string> xTextMapping = null;
		/// <summary>
		/// Gets or sets the text mapping of x label - function that builds text from x-coordinate of mouse in data.
		/// </summary>
		/// <value>The X text mapping.</value>
		public Func<double, string> XTextMapping
		{
			get { return xTextMapping; }
			set
			{
				if (xTextMapping != value)
				{
					xTextMapping = value;
					UpdateUIRepresentation();
				}
			}
		}

		private Func<double, string> yTextMapping = null;
		/// <summary>
		/// Gets or sets the text mapping of y label - function that builds text from y-coordinate of mouse in data.
		/// </summary>
		/// <value>The Y text mapping.</value>
		public Func<double, string> YTextMapping
		{
			get { return yTextMapping; }
			set
			{
				if (yTextMapping != value)
				{
					yTextMapping = value;
					UpdateUIRepresentation();
				}
			}
		}

		private void UpdateUIRepresentation()
		{
			if (Plotter2D != null && Plotter2D.IsHitTestVisible)
			{
				UpdateUIRepresentation(Position);
			}
			else
			{
				circleGrid.Visibility = Visibility.Hidden;
			}
		}

		private void UpdateUIRepresentation(Point mousePosInData)
		{
			if (Plotter2D == null) return;

			var transform = Plotter2D.Viewport.Transform;
			DataRect visible = Plotter2D.Viewport.Visible;
			Rect output = Plotter2D.Viewport.Output;


			Point mousePos = mousePosInData.DataToScreen(transform);
			if (!output.Contains(mousePos))
			{
				if (autoHide)
				{
					circleGrid.Visibility = Visibility.Hidden;
				}
				return;
			}

			var xText = ((Func<string>)(() =>
			{
				double xValue = mousePosInData.X;
				var text = xTextMapping?.Invoke(xValue);

				// doesnot have xTextMapping or it returned null
				text = text ?? GetRoundedValue(visible.XMin, visible.XMax, xValue);

				if (!string.IsNullOrEmpty(customXFormat))
					text = string.Format(customXFormat, text);
				return text;
			})).Invoke();

			var yText = ((Func<string>)(() =>
			{
				double yValue = mousePosInData.Y;
				var text = yTextMapping?.Invoke(yValue);
				// doesnot have xTextMapping or it returned null
				text = text ?? GetRoundedValue(visible.YMin, visible.YMax, yValue);

				if (!string.IsNullOrEmpty(customYFormat))
					text = string.Format(customYFormat, text);
				return text;
			})).Invoke();

			Canvas.SetLeft(circleGrid, mousePos.X);
			Canvas.SetTop(circleGrid, mousePos.Y);

			coordTextBlock.Text = $"{xText},{yText}";
		}

		/// <summary>
		/// Gets or sets the mouse position in screen coordinates.
		/// </summary>
		/// <value>The position.</value>
		public Point Position
		{
			get { return (Point)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}

		/// <summary>
		/// Identifies Position dependency property.
		/// </summary>
		public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
		  "Position",
		  typeof(Point),
		  typeof(CursorNearestPlotGraph),
		  new UIPropertyMetadata(new Point(), OnPositionChanged));

		private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CursorNearestPlotGraph graph = (CursorNearestPlotGraph)d;
			graph.UpdateUIRepresentation((Point)e.NewValue);
		}

		private string GetRoundedValue(double min, double max, double value)
		{
			double roundedValue = value;
			var log = RoundingHelper.GetDifferenceLog(min, max);
			string format = "G3";
			double diff = Math.Abs(max - min);
			if (1E3 < diff && diff < 1E6)
			{
				format = "F0";
			}
			if (log < 0)
				format = "G" + (-log + 2).ToString();

			return roundedValue.ToString(format);
		}

		#region UseDashOffset property

		public bool UseDashOffset
		{
			get { return (bool)GetValue(UseDashOffsetProperty); }
			set { SetValue(UseDashOffsetProperty, value); }
		}

		public static readonly DependencyProperty UseDashOffsetProperty = DependencyProperty.Register(
		  "UseDashOffset",
		  typeof(bool),
		  typeof(CursorNearestPlotGraph),
		  new FrameworkPropertyMetadata(true, UpdateUIRepresentation));

		private static void UpdateUIRepresentation(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CursorNearestPlotGraph graph = (CursorNearestPlotGraph)d;
			if ((bool)e.NewValue)
			{
				graph.UpdateUIRepresentation();
			}
		}

		#endregion

		#region LineStroke property

		public Brush LineStroke
		{
			get { return (Brush)GetValue(LineStrokeProperty); }
			set { SetValue(LineStrokeProperty, value); }
		}

		public static readonly DependencyProperty LineStrokeProperty = DependencyProperty.Register(
		  "LineStroke",
		  typeof(Brush),
		  typeof(CursorNearestPlotGraph),
		  new PropertyMetadata(new SolidColorBrush(Color.FromArgb(130, 0, 0, 0))));

		#endregion

		#region LineStrokeThickness property

		public double LineStrokeThickness
		{
			get { return (double)GetValue(LineStrokeThicknessProperty); }
			set { SetValue(LineStrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty LineStrokeThicknessProperty = DependencyProperty.Register(
		  "LineStrokeThickness",
		  typeof(double),
		  typeof(CursorNearestPlotGraph),
		  new PropertyMetadata(1.0));

		#endregion

	}
}
