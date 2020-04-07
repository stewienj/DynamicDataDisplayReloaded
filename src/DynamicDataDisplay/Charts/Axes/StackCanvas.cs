using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicDataDisplay.Charts
{
	public class StackCanvas : Panel
	{
		public StackCanvas()
		{
			//ClipToBounds = true;
		}

		#region EndCoordinate attached property

		[AttachedPropertyBrowsableForChildren]
		public static double GetEndCoordinate(DependencyObject obj)
		{
			return (double)obj.GetValue(EndCoordinateProperty);
		}

		public static void SetEndCoordinate(DependencyObject obj, double value)
		{
			obj.SetValue(EndCoordinateProperty, value);
		}

		public static readonly DependencyProperty EndCoordinateProperty = DependencyProperty.RegisterAttached(
			"EndCoordinate",
			typeof(double),
			typeof(StackCanvas),
			new PropertyMetadata(double.NaN, OnCoordinateChanged));

		#endregion

		#region Coordinate attached property

		[AttachedPropertyBrowsableForChildren]
		public static double GetCoordinate(DependencyObject obj)
		{
			return (double)obj.GetValue(CoordinateProperty);
		}

		public static void SetCoordinate(DependencyObject obj, double value)
		{
			obj.SetValue(CoordinateProperty, value);
		}

		public static readonly DependencyProperty CoordinateProperty = DependencyProperty.RegisterAttached(
			"Coordinate",
			typeof(double),
			typeof(StackCanvas),
			new PropertyMetadata(0.0, OnCoordinateChanged));

		private static void OnCoordinateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UIElement reference = d as UIElement;
			if (reference != null)
			{
				StackCanvas parent = VisualTreeHelper.GetParent(reference) as StackCanvas;
				if (parent != null)
				{
					parent.InvalidateArrange();
				}
			}
		}
		#endregion

		#region AxisPlacement property

		public AxisPlacement Placement
		{
			get { return (AxisPlacement)GetValue(PlacementProperty); }
			set { SetValue(PlacementProperty, value); }
		}

		public static readonly DependencyProperty PlacementProperty =
			DependencyProperty.Register(
			  "Placement",
			  typeof(AxisPlacement),
			  typeof(StackCanvas),
			  new FrameworkPropertyMetadata(
				  AxisPlacement.Bottom,
				  FrameworkPropertyMetadataOptions.AffectsArrange));

		#endregion

		private bool IsHorizontal
		{
			get { return Placement == AxisPlacement.Top || Placement == AxisPlacement.Bottom; }
		}

		protected override Size MeasureOverride(Size constraint)
		{
			Size availableSize = constraint;
			Size size = new Size();

			bool isHorizontal = IsHorizontal;

			if (isHorizontal)
			{
				availableSize.Width = double.PositiveInfinity;
				size.Width = constraint.Width;
			}
			else
			{
				availableSize.Height = double.PositiveInfinity;
				size.Height = constraint.Height;
			}

			// measuring all children and determinimg self width and height
			foreach (UIElement element in base.Children)
			{
				if (element != null)
				{
					Size childSize = GetChildSize(element, availableSize);
					element.Measure(childSize);
					Size desiredSize = element.DesiredSize;

					if (isHorizontal)
					{
						size.Height = Math.Max(size.Height, desiredSize.Height);
					}
					else
					{
						size.Width = Math.Max(size.Width, desiredSize.Width);
					}
				}
			}

			if (double.IsPositiveInfinity(size.Width)) size.Width = 0;
			if (double.IsPositiveInfinity(size.Height)) size.Height = 0;

			return size;
		}

		private Size GetChildSize(UIElement element, Size availableSize)
		{
			var coordinate = GetCoordinate(element);
			var endCoordinate = GetEndCoordinate(element);

			if (coordinate.IsNotNaN() && endCoordinate.IsNotNaN() && (endCoordinate - coordinate) > 0)
			{
				if (Placement.IsBottomOrTop())
				{
					availableSize.Width = endCoordinate - coordinate;
				}
				else
				{
					availableSize.Height = endCoordinate - coordinate;
				}
			}

			return availableSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			bool isHorizontal = IsHorizontal;

			foreach (FrameworkElement element in base.Children)
			{
				if (element == null)
				{
					continue;
				}

				Size elementSize = element.DesiredSize;
				double x = 0.0;
				double y = 0.0;

				switch (Placement)
				{
					case AxisPlacement.Left:
						x = finalSize.Width - elementSize.Width;
						break;
					case AxisPlacement.Right:
						x = 0;
						break;
					case AxisPlacement.Top:
						y = finalSize.Height - elementSize.Height;
						break;
					case AxisPlacement.Bottom:
						y = 0;
						break;
					default:
						break;
				}

				double coordinate = GetCoordinate(element);

				if (!double.IsNaN(GetEndCoordinate(element)))
				{
					double endCoordinate = GetEndCoordinate(element);
					double size = endCoordinate - coordinate;
					if (size < 0)
					{
						size = -size;
						coordinate -= size;
					}
					if (isHorizontal)
						elementSize.Width = size;
					else
						elementSize.Height = size;
				}


				// shift for common tick labels, not for major ones.
				if (isHorizontal)
				{
					x = coordinate;
					if (element.HorizontalAlignment == HorizontalAlignment.Center)
						x = coordinate - elementSize.Width / 2;
				}
				else
				{
					if (element.VerticalAlignment == VerticalAlignment.Center)
						y = coordinate - elementSize.Height / 2;
					else if (element.VerticalAlignment == VerticalAlignment.Bottom)
						y = coordinate - elementSize.Height;
					else if (element.VerticalAlignment == VerticalAlignment.Top)
						y = coordinate;
				}

				Rect bounds = new Rect(new Point(x, y), elementSize);
				element.Arrange(bounds);
			}

			return finalSize;
		}
	}
}
