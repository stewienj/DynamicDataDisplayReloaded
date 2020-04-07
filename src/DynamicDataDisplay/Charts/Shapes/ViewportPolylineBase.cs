using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.Charts.Shapes
{
	public abstract class ViewportPolylineBase : ViewportShape
	{
		protected ViewportPolylineBase()
		{
		}

		#region Properties

		/// <summary>
		/// Gets or sets the points in Viewport coordinates, that form the line.
		/// </summary>
		/// <value>The points.</value>
		public PointCollection Points
		{
			get { return (PointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}

		/// <summary>
		/// Identifies the Points dependency property.
		/// </summary>
		public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
		  "Points",
		  typeof(PointCollection),
		  typeof(ViewportPolylineBase),
		  new FrameworkPropertyMetadata(new PointCollection(), OnPropertyChanged));

		protected static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ViewportPolylineBase polyline)
			{
				polyline.UpdateUIRepresentation();
			}
		}


		public IEnumerable<Point> PointsSource
		{
			get { return (IEnumerable<Point>)GetValue(PointsSourceProperty); }
			set { SetValue(PointsSourceProperty, value); }
		}

		/// <summary>
		/// Identifies the Points dependency property.
		/// </summary>
		public static readonly DependencyProperty PointsSourceProperty = DependencyProperty.Register(
		  "PointsSource",
		  typeof(IEnumerable<Point>),
		  typeof(ViewportPolylineBase),
		  new FrameworkPropertyMetadata(Enumerable.Empty<Point>(), OnPointsSourcePropertyChanged));

		protected static void OnPointsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ViewportPolylineBase polyline)
			{
				var points = new PointCollection(e.NewValue as IEnumerable<Point> ?? Enumerable.Empty<Point>());
				points.Freeze();
				polyline.Points = points;
			}
		}
		/// <summary>
		/// Gets or sets the fill rule of polygon or polyline.
		/// </summary>
		/// <value>The fill rule.</value>
		public FillRule FillRule
		{
			get { return (FillRule)GetValue(FillRuleProperty); }
			set { SetValue(FillRuleProperty, value); }
		}

		public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register(
		  "FillRule",
		  typeof(FillRule),
		  typeof(ViewportPolylineBase),
		  new FrameworkPropertyMetadata(FillRule.EvenOdd, OnPropertyChanged));

		#endregion

		private PathGeometry geometry = new PathGeometry();
		protected PathGeometry PathGeometry
		{
			get { return geometry; }
		}

		protected sealed override Geometry DefiningGeometry
		{
			get { return geometry; }
		}
	}
}
