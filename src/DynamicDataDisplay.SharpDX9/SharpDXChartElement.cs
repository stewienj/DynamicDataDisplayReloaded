using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Linq;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9
{
	public abstract class SharpDXChartElement : FrameworkElement, IPlotterElement
	{
		private SharpDXHost dxHost;

		protected SharpDXHost DxHost
		{
			get { return dxHost; }
		}

		protected Device Device
		{
			get { return dxHost != null ? dxHost.Device : null; }
		}

		protected Direct3D Direct3D
		{
			get { return dxHost.Direct3D; }
		}

		private void OnDirectXRender(object sender, EventArgs e)
		{
			OnDirectXRender();
		}

		protected virtual void OnDirectXRender() { }

		protected Matrix DxDataTransform { get; private set; } = Matrix.Identity;

		public System.Numerics.Matrix4x4 DataTransform
		{
			get { return (System.Numerics.Matrix4x4)GetValue(DataTransformProperty); }
			set { SetValue(DataTransformProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DataTranform.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DataTransformProperty =
			DependencyProperty.Register("DataTransform", typeof(System.Numerics.Matrix4x4), typeof(SharpDXChartElement), new PropertyMetadata(System.Numerics.Matrix4x4.Identity, (s, e) =>
			{
				if (s is SharpDXChartElement control && e.NewValue is System.Numerics.Matrix4x4 matrix)
				{
					control.DxDataTransform = new Matrix(
						matrix.M11, matrix.M12, matrix.M13, matrix.M14,
						matrix.M21, matrix.M22, matrix.M23, matrix.M24,
						matrix.M31, matrix.M32, matrix.M33, matrix.M34,
						matrix.M41, matrix.M42, matrix.M43, matrix.M44);
				}
			}));


		#region IPlotterElement Members

		private Plotter2D plotter;
		protected Plotter2D Plotter
		{
			get { return plotter; }
		}

		public virtual void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			dxHost = this.plotter.Children.OfType<SharpDXHost>().FirstOrDefault();
			if (dxHost == null)
				throw new InvalidOperationException("First add DirectXHost to plotter.Children");

			dxHost.AddChild(this);
			dxHost.Render += OnDirectXRender;
		}

		public virtual void OnPlotterDetaching(Plotter plotter)
		{
			dxHost.RemoveChild(this);
			dxHost.Render -= OnDirectXRender;
			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
