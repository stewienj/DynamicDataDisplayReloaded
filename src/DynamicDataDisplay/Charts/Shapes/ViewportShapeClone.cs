using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicDataDisplay.Charts
{
	public class ViewportShapeClone : ViewportShape
	{
		/// <summary>
		/// The geometry that gets rendered, this wraps the source geometry
		/// </summary>
		private PathGeometry _geometry;
		/// <summary>
		/// The source that we are copying
		/// </summary>
		private ViewportShape _source;
		/// <summary>
		/// The render offset, relative to the original geometry
		/// </summary>
		private double _xOffset;
		/// <summary>
		/// A tranform that is used to change the location of the geometry
		/// </summary>
		private TranslateTransform _translateTransform;

		public ViewportShapeClone(ViewportShape source, double xOffset)
		{
			_xOffset = xOffset;
			_translateTransform = new TranslateTransform();

			_geometry = new PathGeometry();
			_geometry.Transform = _translateTransform;
			_source = source;
			_source.ShapeGeometry.Changed += (s, e) =>
			{
				_geometry.Clear();
				_geometry.AddGeometry(_source.ShapeGeometry);
			};
			_geometry.Clear();
			_geometry.AddGeometry(_source.ShapeGeometry);

			// The properties we are copying all need to be bindings

			Action<DependencyProperty, string> bind = (dp, path) =>
				   BindingOperations.SetBinding(this, dp, new Binding { Source = _source, Path = new PropertyPath(path), Mode = BindingMode.OneWay });
			bind(StrokeProperty, "Stroke");
			bind(StrokeThicknessProperty, "StrokeThickness");
			bind(StrokeDashArrayProperty, "StrokeDashArray");
			bind(FillProperty, "Fill");
			bind(VisibilityProperty, "Visibility");

			// Need to reflect all mouse events back to the original

			MouseDown += (s, e) => _source.RaiseEvent(e);
			MouseEnter += (s, e) => _source.RaiseEvent(e);
			MouseLeave += (s, e) => _source.RaiseEvent(e);
			MouseLeftButtonDown += (s, e) => _source.RaiseEvent(e);
			MouseLeftButtonUp += (s, e) => _source.RaiseEvent(e);
			MouseMove += (s, e) => _source.RaiseEvent(e);
			MouseRightButtonDown += (s, e) => _source.RaiseEvent(e);
			MouseRightButtonUp += (s, e) => _source.RaiseEvent(e);
			MouseUp += (s, e) => _source.RaiseEvent(e);
			MouseWheel += (s, e) => _source.RaiseEvent(e);

			UpdateUIRepresentationCore();
		}

		/// <summary>
		/// Need to change the transform on the geometry to translate to the new position, this gets called when the viewport changed
		/// </summary>
		protected override void UpdateUIRepresentationCore()
		{
			var viewport = Plotter?.Viewport;
			if (viewport != null)
			{
				var transform = viewport.Transform;

				_translateTransform.X = (transform.DataToScreen(new Point(1, 0)).X - transform.DataToScreen(new Point(0, 0)).X) * _xOffset;
			}
		}

		/// <summary>
		/// Provides the geometry to be rendered
		/// </summary>
		protected override Geometry DefiningGeometry
		{
			get
			{
				return _geometry;
			}
		}
	}
}
