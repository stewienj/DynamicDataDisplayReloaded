using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Common.Auxiliary.DataSearch;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// Adds to ChartPlotter two crossed lines, bound to mouse cursor position, and two labels near axes with mouse position in its text.
	/// </summary>
	public partial class CursorNearestRadioBandLine : CanvasGraph
	{
		private NearestLineSearch2D _nearestPointSearch = new NearestLineSearch2D();
		/// <summary>
		/// Initializes a new instance of the <see cref="CursorNearestRadioBandLine"/> class.
		/// </summary>
		public CursorNearestRadioBandLine()
		{
			_nearestPointSearch.NearestPointUpdated += NearestLineSearch_NearestLineUpdated;
			_nearestPointSearch.NearestVisualSources = new NearestVisualSourceCollection(this);
			_nearestPointSearch.Dispatcher = Dispatcher;
			SnapsToDevicePixels = true;
			InitializeComponent();
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			switch (visualAdded)
			{
				case NearestLineSource lineSource:
					_nearestPointSearch.NearestVisualSources.Add(lineSource);
					break;
				case NearestRectangleSource rectangleSource:
					_nearestPointSearch.NearestVisualSources.Add(rectangleSource);
					break;
			}
			switch (visualRemoved)
			{
				case NearestLineSource lineSource:
					_nearestPointSearch.NearestVisualSources.Remove(lineSource);
					break;
				case NearestRectangleSource rectangleSource:
					_nearestPointSearch.NearestVisualSources.Remove(rectangleSource);
					break;
			}
		}

		private void NearestLineSearch_NearestLineUpdated(object sender, NearestPointSearchArgs e)
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
			if (Plotter2D is RadioBandChartPlotter plotter)
			{
				NearestLineSource lineSource = new NearestLineSource();
				lineSource.ItemsSource = plotter.LineLabelsSource;
				_nearestPointSearch.NearestVisualSources.Add(lineSource);
			}

			UIElement parent = (UIElement)Parent;

			parent.MouseMove += Parent_MouseMove;
			parent.MouseEnter += Parent_MouseEnter;
			parent.MouseLeave += Parent_MouseLeave;
			UpdateUIRepresentation();
		}


		protected override void OnPlotterDetaching()
		{
			UIElement parent = (UIElement)Parent;

			parent.MouseMove -= Parent_MouseMove;
			parent.MouseEnter -= Parent_MouseEnter;
			parent.MouseLeave -= Parent_MouseLeave;
		}

		#endregion

		private void Parent_MouseEnter(object sender, MouseEventArgs e)
		{
			circleGrid.Visibility = Visibility.Visible;
		}

		private void Parent_MouseLeave(object sender, MouseEventArgs e)
		{
			circleGrid.Visibility = Visibility.Hidden;
		}

		private void Parent_MouseMove(object sender, MouseEventArgs e)
		{
			circleGrid.Visibility = Visibility.Visible;
			if (Plotter2D != null && Plotter2D.IsHitTestVisible)
			{
				Point mousePos = Mouse.GetPosition(this);
				CoordinateTransform transform = Plotter2D.Viewport.Transform;
				DataRect visible = Plotter2D.Viewport.Visible;
				Rect output = Plotter2D.Viewport.Output;

				if (!output.Contains(mousePos))
				{
					circleGrid.Visibility = Visibility.Hidden;
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
				circleGrid.Visibility = Visibility.Hidden;
				return;
			}

			var xText = ((Func<string>)(() =>
			{
				double xValue = mousePosInData.X;
				return xValue.ToEngineeringNotation(SignificantFigures) + "Hz";
			})).Invoke();

			var yText = ((Func<string>)(() =>
			{
				double yValue = mousePosInData.Y;
				string text = (Plotter2D as RadioBandChartPlotter)?.GetGroupID(yValue) ?? "";
				return text;
			})).Invoke();

			Canvas.SetLeft(circleGrid, mousePos.X);
			Canvas.SetTop(circleGrid, mousePos.Y);

			coordTextBlock.Text = $"{xText} {yText}";
		}

		public NearestVisualSourceCollection NearestVisualSources
		{
			get
			{
				return _nearestPointSearch.NearestVisualSources;
			}
		}

		public int SignificantFigures { get; set; } = 3;

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
		  typeof(CursorNearestRadioBandLine),
		  new UIPropertyMetadata(new Point(), OnPositionChanged));

		private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CursorNearestRadioBandLine graph = (CursorNearestRadioBandLine)d;
			graph.UpdateUIRepresentation((Point)e.NewValue);
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
		  typeof(CursorNearestRadioBandLine),
		  new FrameworkPropertyMetadata(true, UpdateUIRepresentation));

		private static void UpdateUIRepresentation(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CursorNearestRadioBandLine graph = (CursorNearestRadioBandLine)d;
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
		  typeof(CursorNearestRadioBandLine),
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
		  typeof(CursorNearestRadioBandLine),
		  new PropertyMetadata(1.0));

		#endregion

	}
}
