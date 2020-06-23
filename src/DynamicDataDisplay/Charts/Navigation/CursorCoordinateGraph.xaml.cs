using DynamicDataDisplay.Common;
using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Adds to ChartPlotter two crossed lines, bound to mouse cursor position, and two labels near axes with mouse position in its text.
	/// </summary>
	public partial class CursorCoordinateGraph : ContentGraph
	{
		private CursorCoordinateGraph _linkedHorizontalSource = null;
		private CursorCoordinateGraph _linkedVerticalSource = null;
		private Vector blockShift = new Vector(3, 3);
		/// <summary>
		/// Initializes a new instance of the <see cref="CursorCoordinateGraph"/> class.
		/// </summary>
		public CursorCoordinateGraph()
		{
			SnapsToDevicePixels = true;
			InitializeComponent();
			HorizontalLineStroke = Resources["HorizontalDashedBrush"] as VisualBrush;
			VerticalLineStroke = Resources["VerticalDashedBrush"] as VisualBrush;
			LineStrokeDashArray = null;
		}

		/// <summary>
		/// Gets if the mouse is over the control, returns null if we were
		/// unable to get the mouse status.
		/// </summary>
		/// <remarks>
		/// For some reason MouseLeave was not being fired all the time, IsMouseOver is always true,
		/// and IsMouseDirectlyOver is always false. So I had to do this instead:
		/// </remarks>
		public bool IsMouseOverControl
		{
			get
			{
				var parentWindow = Window.GetWindow(this);
				Point parentMousePos = Mouse.GetPosition(parentWindow);
				var mousePos = parentWindow.TranslatePoint(parentMousePos, this);
				Rect output = Plotter2D.Viewport.Output;
				return output.Contains(mousePos);
			}
		}

		private bool IsMouseOverLinked =>
		  _linkedHorizontalSource?.IsMouseOverControl == true ||
		  _linkedVerticalSource?.IsMouseOverControl == true;

		#region Plotter

		protected override void OnPlotterAttached()
		{
			UIElement parent = (UIElement)Parent;

			parent.MouseMove += parent_MouseMove;
			parent.MouseEnter += Parent_MouseEnter;
			parent.MouseLeave += Parent_MouseLeave;

			UpdateUIRepresentation();
			SetVisibility(Visibility.Hidden);
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
			set
			{
				autoHide = value;
				if (!value)
				{
					SetVisibility(Visibility.Visible);
				}
				else
				{
					if (!IsMouseOverControl)
					{
						SetVisibility(Visibility.Hidden);
					}
				}
			}
		}


		protected void SetVisibility(Visibility visibility)
		{
			switch (visibility)
			{
				case Visibility.Collapsed:
				case Visibility.Hidden:
					horizLine.Visibility = horizGrid.Visibility = Visibility.Hidden;
					vertLine.Visibility = vertGrid.Visibility = Visibility.Hidden;
					break;
				case Visibility.Visible:
					horizLine.Visibility = horizGrid.Visibility = GetHorizontalVisibility();
					vertLine.Visibility = vertGrid.Visibility = GetVerticalVisibility();
					break;
			}
			VisibilityChanged?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler VisibilityChanged;

		private void Parent_MouseEnter(object sender, MouseEventArgs e)
		{
			if (autoHide)
			{
				SetVisibility(Visibility.Visible);
			}
		}

		private void UpdateVisibility(object sender, EventArgs e)
		{
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			if (!autoHide)
			{
				SetVisibility(Visibility.Visible);
			}
			else
			{
				if (!IsMouseOverControl && !IsMouseOverLinked)
				{
					SetVisibility(Visibility.Hidden);
				}
				else
				{
					SetVisibility(Visibility.Visible);
				}
			}
		}

		private Visibility GetHorizontalVisibility()
		{
			return showHorizontalLine ? Visibility.Visible : Visibility.Hidden;
		}

		private Visibility GetVerticalVisibility()
		{
			return showVerticalLine ? Visibility.Visible : Visibility.Hidden;
		}

		private void Parent_MouseLeave(object sender, MouseEventArgs e)
		{
			if (autoHide)
			{
				SetVisibility(Visibility.Hidden);
			}
		}

		private bool followMouse = true;
		/// <summary>
		/// Gets or sets a value indicating whether lines are following mouse cursor position.
		/// </summary>
		/// <value><c>true</c> if lines are following mouse cursor position; otherwise, <c>false</c>.</value>
		public bool FollowMouse
		{
			get { return followMouse; }
			set
			{
				followMouse = value;

				if (!followMouse)
				{
					AutoHide = false;
				}

				UpdateUIRepresentation();
			}
		}

		private void parent_MouseMove(object sender, MouseEventArgs e)
		{
			if (followMouse)
			{
				UpdateUIRepresentation();
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

		private bool showHorizontalLine = true;
		/// <summary>
		/// Gets or sets a value indicating whether to show horizontal line.
		/// </summary>
		/// <value><c>true</c> if horizontal line is shown; otherwise, <c>false</c>.</value>
		public bool ShowHorizontalLine
		{
			get { return showHorizontalLine; }
			set
			{
				if (showHorizontalLine != value)
				{
					showHorizontalLine = value;
					UpdateVisibility();
				}
			}
		}

		private bool showVerticalLine = true;
		/// <summary>
		/// Gets or sets a value indicating whether to show vertical line.
		/// </summary>
		/// <value><c>true</c> if vertical line is shown; otherwise, <c>false</c>.</value>
		public bool ShowVerticalLine
		{
			get { return showVerticalLine; }
			set
			{
				if (showVerticalLine != value)
				{
					showVerticalLine = value;
					UpdateVisibility();
				}
			}
		}

		protected void UpdateUIRepresentation(object sender, EventArgs args)
		{
			UpdateUIRepresentation();
		}

		protected void UpdateUIRepresentation()
		{
			if (Plotter2D != null && Plotter2D.IsHitTestVisible)
			{
				var source = Mouse.PrimaryDevice.ActiveSource;
				if (source != null && !source.IsDisposed)
				{
					var transform = Plotter2D.Viewport.Transform;
					Point position = followMouse ? Mouse.GetPosition(this) : Position;
					//Point mousePosInData = position.ScreenToData(transform);
					UpdateUIRepresentation(position, transform);
				}
			}
			else
			{
				SetVisibility(Visibility.Hidden);
			}
		}



		protected virtual void UpdateUIRepresentation(Point mousePos, CoordinateTransform transform)
		{
			Point mousePosInData = mousePos.ScreenToData(transform);
			UpdateUIRepresentation(mousePosInData, mousePosInData);
		}

		protected void UpdateUIRepresentation(Point mousePosInData, Point cursorPosInData)
		{
			if (Plotter2D == null) return;

			var transform = Plotter2D.Viewport.Transform;
			DataRect visible = Plotter2D.Viewport.Visible;
			Rect output = Plotter2D.Viewport.Output;

			Point mousePos = cursorPosInData.DataToScreen(transform);

			if (!followMouse)
			{
				mousePos = mousePos.DataToScreen(transform);
			}

			// Ensure coordinates are valid
			if (!(mousePos.X.IsFinite() && mousePos.Y.IsFinite()))
				return;

			horizLine.X1 = output.Left;
			horizLine.X2 = output.Right;
			horizLine.Y1 = Math.Floor(mousePos.Y) + 0.5;
			horizLine.Y2 = Math.Floor(mousePos.Y) + 0.5;

			vertLine.X1 = Math.Floor(mousePos.X) + 0.5;
			vertLine.X2 = Math.Floor(mousePos.X) + 0.5;
			vertLine.Y1 = output.Top;
			vertLine.Y2 = output.Bottom;

			if (UseDashOffset)
			{
				horizLine.StrokeDashOffset = (output.Right - mousePos.X) / 2;
				vertLine.StrokeDashOffset = (output.Bottom - mousePos.Y) / 2;
			}

			string text = null;

			if (showVerticalLine)
			{
				double xValue = cursorPosInData.X;
				if (xTextMapping != null)
					text = xTextMapping(xValue);

				// doesnot have xTextMapping or it returned null
				if (text == null)
					text = GetRoundedValue(visible.XMin, visible.XMax, xValue);

				if (!string.IsNullOrEmpty(customXFormat))
					text = string.Format(customXFormat, text);
				horizTextBlock.Text = text;
				// Set opacitiy not visiblity to avoid conflicting with the visibility settings
				horizGrid.Opacity = string.IsNullOrWhiteSpace(text) ? 0 : 1.0;
			}

			double width = horizGrid.ActualWidth;
			double x = mousePos.X + blockShift.X;
			if (x + width > output.Right)
			{
				x = mousePos.X - blockShift.X - width;
			}
			Canvas.SetLeft(horizGrid, x);

			if (showHorizontalLine)
			{
				double yValue = cursorPosInData.Y;
				text = null;
				if (yTextMapping != null)
					text = yTextMapping(yValue);

				if (text == null)
					text = GetRoundedValue(visible.YMin, visible.YMax, yValue);

				if (!string.IsNullOrEmpty(customYFormat))
					text = string.Format(customYFormat, text);
				vertTextBlock.Text = text;
				// Set opacitiy not visiblity to avoid conflicting with the visibility settings
				vertGrid.Opacity = string.IsNullOrWhiteSpace(text) ? 0 : 1.0;
			}

			// by default vertGrid is positioned on the top of line.
			double height = vertGrid.ActualHeight;
			double y = mousePos.Y - blockShift.Y - height;
			if (y < output.Top)
			{
				y = mousePos.Y + blockShift.Y;
			}
			Canvas.SetTop(vertGrid, y);

			if (followMouse)
				Position = mousePos;
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
		  typeof(CursorCoordinateGraph),
		  new UIPropertyMetadata(new Point(), OnPositionChanged));

		private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CursorCoordinateGraph graph = (CursorCoordinateGraph)d;
			graph.UpdateUIRepresentation();
			graph.PositionChanged?.Invoke(graph, EventArgs.Empty);
		}

		public event EventHandler PositionChanged;

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
		  typeof(CursorCoordinateGraph),
		  new FrameworkPropertyMetadata(true, UpdateUIRepresentation));

		private static void UpdateUIRepresentation(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CursorCoordinateGraph graph = (CursorCoordinateGraph)d;
			if ((bool)e.NewValue)
			{
				graph.UpdateUIRepresentation();
			}
			else
			{
				graph.vertLine.ClearValue(Line.StrokeDashOffsetProperty);
				graph.horizLine.ClearValue(Line.StrokeDashOffsetProperty);
			}
		}

		#endregion

		#region HorizontalLineStroke property

		public Brush HorizontalLineStroke
		{
			get { return (Brush)GetValue(HorizontalLineStrokeProperty); }
			set { SetValue(HorizontalLineStrokeProperty, value); }
		}

		public static readonly DependencyProperty HorizontalLineStrokeProperty = DependencyProperty.Register(
		  "HorizontalLineStroke",
		  typeof(Brush),
		  typeof(CursorCoordinateGraph),
		  new PropertyMetadata(new SolidColorBrush(Color.FromArgb(130, 0, 0, 0))));

		#endregion

		#region VerticalLineStroke property

		public Brush VerticalLineStroke
		{
			get { return (Brush)GetValue(VerticalLineStrokeProperty); }
			set { SetValue(VerticalLineStrokeProperty, value); }
		}

		public static readonly DependencyProperty VerticalLineStrokeProperty = DependencyProperty.Register(
		  "VerticalLineStroke",
		  typeof(Brush),
		  typeof(CursorCoordinateGraph),
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
		  typeof(CursorCoordinateGraph),
		  new PropertyMetadata(1.0));

		#endregion

		#region LineStrokeDashArray property

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public DoubleCollection LineStrokeDashArray
		{
			get { return (DoubleCollection)GetValue(LineStrokeDashArrayProperty); }
			set { SetValue(LineStrokeDashArrayProperty, value); }
		}

		public static readonly DependencyProperty LineStrokeDashArrayProperty = DependencyProperty.Register(
		  "LineStrokeDashArray",
		  typeof(DoubleCollection),
		  typeof(CursorCoordinateGraph),
		  new FrameworkPropertyMetadata(DoubleCollectionHelper.Create(4, 2)));

		#endregion


		private void LinkToSource(CursorCoordinateGraph source, Orientation orientation)
		{
			switch (orientation)
			{
				case Orientation.Horizontal:
					if (_linkedHorizontalSource != null)
					{
						_linkedHorizontalSource.VisibilityChanged -= UpdateVisibility;
						_linkedHorizontalSource.PositionChanged -= UpdateUIRepresentation;
					}
					_linkedHorizontalSource = source;
					if (_linkedHorizontalSource != null)
					{
						_linkedHorizontalSource.VisibilityChanged += UpdateVisibility;
						_linkedHorizontalSource.PositionChanged += UpdateUIRepresentation;
					}
					break;
				case Orientation.Vertical:
					if (_linkedVerticalSource != null)
					{
						_linkedVerticalSource.VisibilityChanged -= UpdateVisibility;
						_linkedVerticalSource.PositionChanged -= UpdateUIRepresentation;
					}
					_linkedVerticalSource = source;
					if (_linkedVerticalSource != null)
					{
						_linkedVerticalSource.VisibilityChanged += UpdateVisibility;
						_linkedVerticalSource.PositionChanged += UpdateUIRepresentation;
					}
					break;
			}
		}


		public CursorCoordinateGraph LinkVerticalSource
		{
			get { return (CursorCoordinateGraph)GetValue(LinkXSourceProperty); }
			set { SetValue(LinkXSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LinkXSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LinkXSourceProperty =
			DependencyProperty.Register("LinkVerticalSource", typeof(CursorCoordinateGraph), typeof(CursorCoordinateGraph), new PropertyMetadata(null, (d, e) =>
			 {
				 if (d is CursorCoordinateGraph control)
				 {
					 control.LinkToSource(e.NewValue as CursorCoordinateGraph, Orientation.Vertical);
				 }
			 }));


		public CursorCoordinateGraph LinkHorizontalSource
		{
			get { return (CursorCoordinateGraph)GetValue(LinkYSourceProperty); }
			set { SetValue(LinkYSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LinkXSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LinkYSourceProperty =
			DependencyProperty.Register("LinkHorizontalSource", typeof(CursorCoordinateGraph), typeof(CursorCoordinateGraph), new PropertyMetadata(null, (d, e) =>
			{
				if (d is CursorCoordinateGraph control)
				{
					control.LinkToSource(e.NewValue as CursorCoordinateGraph, Orientation.Horizontal);
				}
			}));


	}
}
